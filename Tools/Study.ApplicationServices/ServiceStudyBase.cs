﻿using System.Collections.Generic;
using Study.Util;
using Study.Domains.Framework;
using Study.Domains.Framework.Repositories;
using Study.MyBatis;
using Study.MyBatis.Service;
using EmitMapper;
using EmitMapper.MappingConfiguration;
using AopHelper;

namespace Study.ApplicationServices
{
    public abstract class ServiceStudyBase<TEntity, TKey, TDto, TQuery> : SqlServerSevice, IServiceStudyBase<TDto, TQuery>
        where TEntity : class, IAggregateRoot<TKey>
        where TDto : new()
        where TQuery : IPager
    {
        #region 待处理
        protected ServiceStudyBase()
        {
            //UnitOfWork = unitOfWork;
        }

        protected virtual TDto ToDto(TEntity entity)
        {
            //默认的成员对应
            var mapper = new ObjectMapperManager().GetMapper<TEntity, TDto>(new DefaultMapConfig());
            TDto dto = mapper.Map(entity);
            return dto;
        }

        protected virtual TEntity ToEntity(TDto dto)
        {
            //默认的成员对应
            var mapper = new ObjectMapperManager().GetMapper<TDto, TEntity>(new DefaultMapConfig());
            TEntity entity = mapper.Map(dto);
            return entity;
        }
        #endregion               

        #region 增加 删除 更新 查询单个数据

        public virtual TDto Create()
        {
            return new TDto();
        }

        public virtual void Add(TDto parameter)
        {
            var entity = ToEntity(parameter);
            string insertId = GetInsertSqlId();
            Insert(insertId, entity);
        }

        [LoggingAspect]
        private void Insert(string insertId, TEntity entity)
        {
            DataMapper.Insert(insertId, entity);
        }

        
        public virtual void Delete(string id)
        {
            string deleteId = GetDeleteSqlId();
            DataMapper.Delete(deleteId, id);
        }

        [LoggingAspect]
        private void Delete(string deleteId, string  id)
        {
            DataMapper.Delete(deleteId, id);
        }

        public virtual int Update(TDto parameter)
        {
            var entity = ToEntity(parameter);
            string updateId = GetUpdateSqlId();
            return Update(updateId, entity);
        }

        [LoggingAspect]
        private int Update(string updateId, TEntity entity)
        {
            return DataMapper.Update(updateId, entity);
        }

        public virtual TDto FetchOne(string id)
        {
            string fetchId = GetQuerySqlWithParameterIsId();
            return DataMapper.QueryForObject<TDto>(fetchId, id);
        }

        #endregion

        #region 批量增加 删除 更新 查询数据

        public virtual void BatchAdd(IList<TDto> parameters)
        {
            foreach (var item in parameters)
            {
                Add(item);
            }
        }

        public virtual void BatchDelete(string ids)
        {
            var idList = Conv.ToList<string>(ids);
            foreach (var id in idList)
            {
                Delete(id);
            }
        }

        public virtual IList<TDto> FetchMany(string ids)
        {
            string fetchIds = GetQuerySqlWithParameterIsIds();
            return DataMapper.QueryForList<TDto>(fetchIds, ids);
        }
        
        public virtual PagerList<TDto> FetchPages(TQuery query)
        {
            string fetchQuery = GetQuerySqlId();
            int total = 0;
            int wiilShowedPage = query.Page;
            int pageSize = query.PageSize;
            IList<TDto> list = DataMapper.QueryForListWithPage<TDto>(fetchQuery, query, query.Order, wiilShowedPage, pageSize, ref total);
            query.TotalCount = total;
            PagerList<TDto> pagers = new PagerList<TDto>(query);
            pagers.AddRange(list);
            return pagers;
        }

        public virtual IList<TDto> FetchAll(TQuery query)
        {
            string fetchQuery = GetQuerySqlId();
            return DataMapper.QueryForList<TDto>(fetchQuery, query);
        }

        public virtual IList<TDto> FetchAll()
        {
            string fetchQuery = GetQuerySqlId();
            return DataMapper.QueryForList<TDto>(fetchQuery, null);
        }

        #endregion

        #region 获取SQL语句对应的ID

        public virtual string GetInsertSqlId()
        {
            return string.Empty;
        }

        public virtual string GetDeleteSqlId()
        {
            return string.Empty;
        }

        public virtual string GetUpdateSqlId()
        {
            return string.Empty;
        }
        public virtual string GetQuerySqlId()
        {
            return string.Empty;
        }

        public virtual string GetQuerySqlWithParameterIsId()
        {
            return string.Empty;
        }

        public virtual string GetQuerySqlWithParameterIsIds()
        {
            return string.Empty;
        }



        #endregion
    }
}
