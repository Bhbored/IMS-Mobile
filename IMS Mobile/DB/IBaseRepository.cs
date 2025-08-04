using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IMS_Mobile.MVVM.Models;


namespace IMS_Mobile.DB
{
    public interface IBaseRepository<T> : IDisposable
        where T : Entity, new()
    {
        #region Connection
        void StopConnection();                   // explicit close
        #endregion

        #region Insert
        int InsertItem(T item);
        int InsertItems(IEnumerable<T> items);
        void InsertItemWithChildren(T item, bool recursive = false);
        void InsertItemWithExistingChildren<TChild>(
            T parent,
            IEnumerable<TChild> existingChildren,
            Action<T, IEnumerable<TChild>> assignRelation)
            where TChild : Entity, new();
        #endregion

        #region Update
        int UpdateItem(T item);
        int UpdateItems(IEnumerable<T> items);
        void UpdateItemWithChildren(T item, bool recursive = false);
        void ReplaceChildren<TChild>(
            T parent,
            IEnumerable<TChild> newChildren,
            Action<T, IEnumerable<TChild>> assignRelation)
            where TChild : Entity, new();
        void AddNewChildToParent<TChild>(
            T parent,
            TChild newChild,
            Action<T, IEnumerable<TChild>> assignRelation)
            where TChild : Entity, new();
        void RemoveChildFromParent<TChild>(
            T parent,
            TChild child,
            Action<T, IEnumerable<TChild>> assignRelation)
            where TChild : Entity, new();
        #endregion

        #region Delete
        void DeleteItem(T item, bool withChildren = false);
        void DeleteChild<TChild>(TChild child) where TChild : Entity, new();
        #endregion

        #region Select
        T GetItem(int id);
        T GetItem(Expression<Func<T, bool>> predicate);
        List<T> GetItems();
        List<T> GetItems(Expression<Func<T, bool>> predicate);
        List<T> GetItemsWithChildren();
        #endregion
    }
}
