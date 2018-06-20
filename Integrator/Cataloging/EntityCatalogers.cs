using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Cataloging
{
    /// <summary>
    /// Maps truth table of entities, based on whether keys are shared with foreign entities
    /// </summary>
    /// <typeparam name="TInternal">Internal entity</typeparam>
    /// <typeparam name="TExternal">External entity</typeparam>
    /// <typeparam name="TSharedKey">Key shared by both internal and external entity</typeparam>
    public abstract class EntityCataloger<TInternal, TExternal, TSharedKey>
        //where TInternal : class
        //where TExternal : class
        //where TSharedKey : class
    {
        //protected readonly Dictionary<TSharedKey, TExternal> _externalEntities;
        //protected readonly IEnumerable<TInternal> _internalEntities;
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="externalEntities">Use a pre-initialized dictionary</param>
        ///// <param name="internalEntities"></param>
        ///// /// <exception cref="ArgumentNullException">If any arguments are null</exception>
        //public EntityCataloger(Dictionary<TSharedKey, TExternal> externalEntities, IEnumerable<TInternal> internalEntities)
        //{
        //    if (externalEntities == null) throw new ArgumentNullException(nameof(externalEntities));
        //    if (internalEntities == null) throw new ArgumentNullException(nameof(internalEntities));
        //    this._externalEntities = externalEntities;
        //    this._internalEntities = internalEntities;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="externalEntities"></param>
        ///// <param name="internalEntities"></param>
        ///// <exception cref="ArgumentNullException">If any arguments are null</exception>
        //public EntityCataloger(IEnumerable<TExternal> externalEntities, IEnumerable<TInternal> internalEntities)
        //{
        //    if (externalEntities == null) throw new ArgumentNullException(nameof(externalEntities));
        //    if (internalEntities == null) throw new ArgumentNullException(nameof(internalEntities));
        //    this._externalEntities = externalEntities.Where(x => GetExternalKey(x) != null).ToDictionary(x => GetExternalKey(x));
        //    this._internalEntities = internalEntities;
        //}

        public event EventHandler<TInternal> InternalNullKeySkipped;
        public event EventHandler<TExternal> ExternalNullKeySkipped;

        protected virtual void OnInternalNullKeySkipped(TInternal e)
        {
            InternalNullKeySkipped?.Invoke(this, e);
        }

        protected virtual void OnExternalNullKeySkipped(TExternal e)
        {
            ExternalNullKeySkipped?.Invoke(this, e);
        }

        /// <summary>
        /// Catalogs the initially provided entities.
        /// </summary>
        /// <param name="externalEntities"></param>
        /// <param name="internalEntities"></param>
        /// <returns></returns>
        public EntityCatalogResult<TInternal, TExternal> Map(IEnumerable<TExternal> externalEntities, IEnumerable<TInternal> internalEntities)
        {
            if (externalEntities == null) throw new ArgumentNullException(nameof(externalEntities));
            if (internalEntities == null) throw new ArgumentNullException(nameof(internalEntities));
            return Map(externalEntities.Where(x => GetExternalKey(x) != null).ToDictionary(x => GetExternalKey(x)), internalEntities);
        }

        /// <summary>
        /// Catalogs the initially provided entities.
        /// </summary>
        /// <param name="externalEntities"></param>
        /// <param name="internalEntities"></param>
        /// <returns></returns>
        public EntityCatalogResult<TInternal, TExternal> Map(Dictionary<TSharedKey, TExternal> externalEntities, IEnumerable<TInternal> internalEntities)
        {
            if (externalEntities == null) throw new ArgumentNullException(nameof(externalEntities));
            if (internalEntities == null) throw new ArgumentNullException(nameof(internalEntities));
            var result = new EntityCatalogResult<TInternal, TExternal>();

            var externalEntitiesNotPresentInternally = new Dictionary<TSharedKey, TExternal>(externalEntities);
            foreach (var internalEntity in internalEntities)
            {
                TSharedKey keyFromInternal = GetInternalKey(internalEntity);
                if (keyFromInternal == null)
                {
                    OnInternalNullKeySkipped(internalEntity);
                    continue;
                }
                TExternal externalEntity;
                if (!externalEntities.TryGetValue(keyFromInternal, out externalEntity))
                {
                    result.ExistsOnlyInternally.Add(internalEntity);
                }
                else
                {
                    externalEntitiesNotPresentInternally.Remove(keyFromInternal);
                    result.ExistsEverywhere.Add(new Tuple<TInternal, TExternal>(internalEntity, externalEntity));
                }
            }
            foreach (TExternal externalEntity in externalEntitiesNotPresentInternally.Values)
            {
                result.ExistsOnlyExternally.Add(externalEntity);
            }

            return result;
        }
        public abstract TSharedKey GetInternalKey(TInternal internalEntity);
        public abstract TSharedKey GetExternalKey(TExternal externalEntity);
    }

    public class EntityCatalogResult<TInternal, TExternal>
    {
        public readonly IList<TInternal> ExistsOnlyInternally;
        public readonly IList<TExternal> ExistsOnlyExternally;
        public readonly IList<Tuple<TInternal, TExternal>> ExistsEverywhere;

        public EntityCatalogResult()
        {
            ExistsOnlyInternally = new List<TInternal>();
            ExistsOnlyExternally = new List<TExternal>();
            ExistsEverywhere = new List<Tuple<TInternal, TExternal>>();
        }
    }

    //With mirror
    /// <summary>
    /// Maps truth table of entities, based on whether keys are shared with foreign entities
    /// </summary>
    /// <typeparam name="TInternal"></typeparam>
    /// <typeparam name="TMirror">Entity serving as a copy the external entity, to help keeping track of states of transactions in certain environments. (Such as a non-request-based service that syncs with a state-oblivious external API)</typeparam>
    /// <typeparam name="TExternal"></typeparam>
    /// <typeparam name="TSharedKey"></typeparam>
    public abstract class EntityCataloger<TInternal, TMirror, TExternal, TSharedKey>
       //where TInternal : class
       // where TMirror : class
       //where TExternal : class
       //where TSharedKey : class
    {
        //protected readonly Dictionary<TSharedKey, TExternal> _externalEntities;
        //protected readonly Dictionary<TSharedKey, TMirror> _mirrorEntities;
        //protected readonly IEnumerable<TInternal> _internalEntities;

        //public EntityCataloger(Dictionary<TSharedKey, TExternal> externalEntities, Dictionary<TSharedKey, TMirror> mirrorEntities, IEnumerable<TInternal> internalEntities)
        //{
        //    if (externalEntities == null) throw new ArgumentNullException(nameof(externalEntities));
        //    if (mirrorEntities == null) throw new ArgumentNullException(nameof(mirrorEntities));
        //    if (internalEntities == null) throw new ArgumentNullException(nameof(internalEntities));
        //    this._externalEntities = externalEntities;
        //    this._mirrorEntities = mirrorEntities;
        //    this._internalEntities = internalEntities;
        //}

        //public EntityCataloger(IEnumerable<TExternal> externalEntities, IEnumerable<TMirror> mirrorEntities, IEnumerable<TInternal> internalEntities)
        //{
        //    if (externalEntities == null) throw new ArgumentNullException(nameof(externalEntities));
        //    if (mirrorEntities == null) throw new ArgumentNullException(nameof(mirrorEntities));
        //    if (internalEntities == null) throw new ArgumentNullException(nameof(internalEntities));
        //    this._externalEntities = externalEntities
        //        .Where(x => GetExternalKey(x) != null)
        //            .ToDictionary(x => GetExternalKey(x));
        //    this._mirrorEntities = mirrorEntities
        //        .Where(x => GetMirrorKey(x) != null)
        //            .ToDictionary(x => GetMirrorKey(x));
        //    this._internalEntities = internalEntities;
        //}

        public event EventHandler<TInternal> InternalNullKeySkipped;
        public event EventHandler<TExternal> ExternalNullKeySkipped;
        public event EventHandler<TMirror> MirrorNullKeySkipped;

        protected virtual void OnInternalNullKeySkipped(TInternal e)
        {
            InternalNullKeySkipped?.Invoke(this, e);
        }

        protected virtual void OnExternalNullKeySkipped(TExternal e)
        {
            ExternalNullKeySkipped?.Invoke(this, e);
        }

        protected virtual void OnMirrorNullKeySkipped(TMirror e)
        {
            MirrorNullKeySkipped?.Invoke(this, e);
        }

        //public Func<TInternal, bool> ShouldSkipInternalEntity { get; set; }
        //public Func<TMirror, bool> ShouldSkipMirrorEntity { get; set; }
        //public Func<TExternal, bool> ShouldSkipExternalEntity { get; set; }

        public EntityCatalogResult<TInternal, TMirror, TExternal> Map(IEnumerable<TExternal> externalEntities, IEnumerable<TMirror> mirrorEntities, IEnumerable<TInternal> internalEntities)
        {
            if (externalEntities == null) throw new ArgumentNullException(nameof(externalEntities));
            if (mirrorEntities == null) throw new ArgumentNullException(nameof(mirrorEntities));
            if (internalEntities == null) throw new ArgumentNullException(nameof(internalEntities));
            var dictExtEntities = externalEntities
                .Where(x => GetExternalKey(x) != null)
                    .ToDictionary(x => GetExternalKey(x));
            var dictMirrorEntities = mirrorEntities
                .Where(x => GetMirrorKey(x) != null)
                    .ToDictionary(x => GetMirrorKey(x));
            return Map(dictExtEntities, dictMirrorEntities, internalEntities);
        }


        public EntityCatalogResult<TInternal, TMirror, TExternal> Map(Dictionary<TSharedKey, TExternal> externalEntities, Dictionary<TSharedKey, TMirror> mirrorEntities, IEnumerable<TInternal> internalEntities)
        {
            if (externalEntities == null) throw new ArgumentNullException(nameof(externalEntities));
            if (mirrorEntities == null) throw new ArgumentNullException(nameof(mirrorEntities));
            if (internalEntities == null) throw new ArgumentNullException(nameof(internalEntities));

            var result = new EntityCatalogResult<TInternal, TMirror, TExternal>();

            var externalEntitiesNotPresentElsewhere = new Dictionary<TSharedKey, TExternal>(externalEntities);
            var mirrorEntitiesNotPresentInternally = new Dictionary<TSharedKey, TMirror>(mirrorEntities);
            Dictionary<TSharedKey, TInternal> internalEntitiesNotPresentInMirror =
                new Dictionary<TSharedKey, TInternal>(
                    internalEntities
                    .Where(x => GetInternalKey(x) != null)
                    .ToDictionary(x => GetInternalKey(x)));
            foreach (var internalEntity in internalEntities)
            {
                TSharedKey keyFromInternal = GetInternalKey(internalEntity);
                if (keyFromInternal == null)
                {
                    OnInternalNullKeySkipped(internalEntity);
                    continue;
                }
                TMirror mirrorEntity;
                if (mirrorEntities.TryGetValue(keyFromInternal, out mirrorEntity))
                {
                    mirrorEntitiesNotPresentInternally.Remove(keyFromInternal);
                    internalEntitiesNotPresentInMirror.Remove(keyFromInternal);
                    TExternal externalEntity;
                    if (!externalEntities.TryGetValue(keyFromInternal, out externalEntity))
                    {
                        result.ExistsInternallyAndMirror.Add(new Tuple<TInternal, TMirror>(internalEntity, mirrorEntity));
                    }
                    else
                    {
                        externalEntitiesNotPresentElsewhere.Remove(keyFromInternal);
                        result.ExistsEverywhere.Add(new Tuple<TInternal, TMirror, TExternal>(internalEntity, mirrorEntity, externalEntity));
                    }
                }
                else
                {
                    TExternal externalEntity;
                    if (!externalEntities.TryGetValue(keyFromInternal, out externalEntity))
                    {
                        result.ExistsOnlyInternally.Add(internalEntity);
                    }
                    else
                    {
                        externalEntitiesNotPresentElsewhere.Remove(keyFromInternal);
                        result.ExistsInternallyAndExternally.Add(new Tuple<TInternal, TExternal>(internalEntity, externalEntity));
                    }
                }
            }
            foreach (TMirror mirrorEntity in mirrorEntitiesNotPresentInternally.Values)
            {
                TSharedKey keyFromMirror = GetMirrorKey(mirrorEntity);
                if (keyFromMirror == null)
                {
                    OnMirrorNullKeySkipped(mirrorEntity);
                    continue;
                }
                TExternal externalEntity;
                if (!externalEntities.TryGetValue(keyFromMirror, out externalEntity))
                {
                    result.ExistsOnlyInMirror.Add(mirrorEntity);
                }
                else
                {
                    externalEntitiesNotPresentElsewhere.Remove(keyFromMirror);
                    result.ExistsInMirrorAndExternally.Add(new Tuple<TMirror, TExternal>(mirrorEntity, externalEntity));
                }
            }
            foreach (TExternal externalEntity in externalEntitiesNotPresentElsewhere.Values)
            {
                result.ExistsOnlyExternally.Add(externalEntity);
            }

            return result;
        }
        public abstract TSharedKey GetInternalKey(TInternal internalEntity);
        public abstract TSharedKey GetMirrorKey(TMirror mirrorEntity);
        public abstract TSharedKey GetExternalKey(TExternal externalEntity);
    }


    public class EntityCatalogResult<TInternal, TMirror, TExternal>
    {
        public EntityCatalogResult()
        {
            ExistsOnlyInternally = new List<TInternal>();
            ExistsOnlyExternally = new List<TExternal>();
            ExistsOnlyInMirror = new List<TMirror>();
            ExistsInternallyAndMirror = new List<Tuple<TInternal, TMirror>>();
            ExistsInternallyAndExternally = new List<Tuple<TInternal, TExternal>>();
            ExistsInMirrorAndExternally = new List<Tuple<TMirror, TExternal>>();
            ExistsEverywhere = new List<Tuple<TInternal, TMirror, TExternal>>();
        }

        public readonly List<TInternal> ExistsOnlyInternally;
        public readonly List<TExternal> ExistsOnlyExternally;
        public readonly List<TMirror> ExistsOnlyInMirror;
        public readonly List<Tuple<TInternal, TMirror>> ExistsInternallyAndMirror;
        public readonly List<Tuple<TInternal, TExternal>> ExistsInternallyAndExternally;
        public readonly List<Tuple<TMirror, TExternal>> ExistsInMirrorAndExternally;
        public readonly List<Tuple<TInternal, TMirror, TExternal>> ExistsEverywhere;
    }
}
