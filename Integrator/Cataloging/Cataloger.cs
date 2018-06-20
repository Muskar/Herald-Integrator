using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Cataloging
{
    /// <summary>
    /// Simple generic entity cataloger using func's to get keys
    /// </summary>
    /// <typeparam name="TInternal"></typeparam>
    /// <typeparam name="TExternal"></typeparam>
    /// <typeparam name="TSharedKey"></typeparam>
    public class Cataloger<TInternal, TExternal, TSharedKey> : EntityCataloger<TInternal, TExternal, TSharedKey>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="getInternalKey">Function to get key. Must not be null</param>
        /// <param name="getExternalKey">Function to get key. Must not be null</param>
        /// /// <exception cref="ArgumentNullException">If either argument is null</exception>
        public Cataloger(Func<TInternal, TSharedKey> getInternalKey, Func<TExternal, TSharedKey> getExternalKey)
        {
            _getInternalKey = getInternalKey ?? throw new ArgumentNullException(nameof(getInternalKey));
            _getExternalKey = getExternalKey ?? throw new ArgumentNullException(nameof(getExternalKey));
        }

        private Func<TInternal, TSharedKey> _getInternalKey { get; }
        private Func<TExternal, TSharedKey> _getExternalKey { get; }

        /// <summary>
        /// Gets shared key from the external entity
        /// </summary>
        /// <param name="externalEntity"></param>
        /// /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public override TSharedKey GetExternalKey(TExternal externalEntity)
        {
            if (externalEntity == null) throw new ArgumentNullException(nameof(externalEntity));
            return _getExternalKey(externalEntity);
        }
        /// <summary>
        /// Gets shared key from the internal entity
        /// </summary>
        /// <param name="internalEntity"></param>
        /// /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public override TSharedKey GetInternalKey(TInternal internalEntity)
        {
            if (internalEntity == null) throw new ArgumentNullException(nameof(internalEntity));
            return _getInternalKey(internalEntity);
        }
    }

    /// <summary>
    /// Simple generic entity cataloger using func's to get keys
    /// </summary>
    /// <typeparam name="TInternal"></typeparam>
    /// <typeparam name="TMirror">Entity serving as a copy the external entity, to help keeping track of states of transactions in certain environments. (Such as a non-request-based service that syncs with a state-oblivious external API)</typeparam>
    /// <typeparam name="TExternal"></typeparam>
    /// <typeparam name="TSharedKey"></typeparam>
    public class Cataloger<TInternal, TMirror, TExternal, TSharedKey> : EntityCataloger<TInternal, TMirror, TExternal, TSharedKey>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="getInternalKey">Function to get key. Must not be null</param>
        /// <param name="getMirrorKey">Function to get key. Must not be null</param>
        /// <param name="getExternalKey">Function to get key. Must not be null</param>
        /// /// <exception cref="ArgumentNullException">If either argument is null</exception>
        public Cataloger(Func<TInternal, TSharedKey> getInternalKey, Func<TMirror, TSharedKey> getMirrorKey, Func<TExternal, TSharedKey> getExternalKey)
        {
            _getInternalKey = getInternalKey ?? throw new ArgumentNullException(nameof(getInternalKey));
            _getMirrorKey = getMirrorKey ?? throw new ArgumentNullException(nameof(getMirrorKey));
            _getExternalKey = getExternalKey ?? throw new ArgumentNullException(nameof(getExternalKey));
        }

        private Func<TInternal, TSharedKey> _getInternalKey { get; }
        private Func<TMirror, TSharedKey> _getMirrorKey { get; }
        private Func<TExternal, TSharedKey> _getExternalKey { get; }

        /// <summary>
        /// Gets shared key from external entity
        /// </summary>
        /// <param name="externalEntity"></param>
        /// /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public override TSharedKey GetExternalKey(TExternal externalEntity)
        {
            if (externalEntity == null) throw new ArgumentNullException(nameof(externalEntity));
            return _getExternalKey(externalEntity);
        }

        /// <summary>
        /// Gets shared key from internal entity
        /// </summary>
        /// <param name="internalEntity"></param>
        /// /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public override TSharedKey GetInternalKey(TInternal internalEntity)
        {
            if (internalEntity == null) throw new ArgumentNullException(nameof(internalEntity));
            return _getInternalKey(internalEntity);
        }

        /// <summary>
        /// Gets shared key from mirror entity
        /// </summary>
        /// <param name="mirrorEntity"></param>
        /// /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public override TSharedKey GetMirrorKey(TMirror mirrorEntity)
        {
            if (mirrorEntity == null) throw new ArgumentNullException(nameof(mirrorEntity));
            return _getMirrorKey(mirrorEntity);
        }
    }
}
