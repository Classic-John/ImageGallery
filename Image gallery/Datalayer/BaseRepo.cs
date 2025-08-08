using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models.BaseClass;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Core
{
    public abstract class BaseRepo<T> where T : BaseEntity
    {
        private DbSet<T> _dbSet;
        private List<T> _items;
        public BaseRepo()
        {

        }
        public void Initialise(ImageGalleryContext context)
        {
            _dbSet = context.Set<T>();
            _items = _dbSet.Select(theItem => theItem).ToListAsync().Result;
        }
        public List<T> GetItems()
            => _items;
        public async Task<bool> AddItem(T item)
        {
            try
            {
                _dbSet.Add(item);
                _items.Add(item);
                await _dbSet.GetDbContext().SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }
            return true;
        }
        public async Task<bool> UpdateItem(T itemInQuestion)
        {
            try
            {
                T? found = _items.Find(item => item.Id == itemInQuestion.Id);
                _dbSet.GetDbContext().Entry(found).CurrentValues.SetValues(found);
                _dbSet.Update(found);
                await _dbSet.GetDbContext().SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }
            return true;
        }
        public async Task<bool> DeleteItem(T itemInQuestion)
        {
            try
            {
                _dbSet.Remove(itemInQuestion);
                _items.Remove(itemInQuestion);
                await _dbSet.GetDbContext().SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }
            return true;
        }
    }
}
