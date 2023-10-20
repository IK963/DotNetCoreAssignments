using DotNetCoreAssignments.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ToDoRepository
{
    #region members

    private readonly ApplicationDbContext _context;

    #endregion

    public ToDoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ToDo> GetAll()
    {
        return _context.ToDo.ToList(); 
    }
    
    public ToDo GetById(Guid id)
    {
        return _context.ToDo.FirstOrDefault(ToDo => ToDo.Id == id) ?? null;
    }

    public ToDo Create(ToDo item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(ToDo));
        }
        try
        {
            item.Id = Guid.NewGuid();

            if (_context.ToDo != null)
                _context.ToDo.Add(item);

            _context.SaveChanges(); // Save the changes to the database
            return item;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    
    public void Update(ToDo item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        try
        {
            if (_context.ToDo != null)
            {
                var toDoItem = _context.ToDo.FirstOrDefault(ToDo => ToDo.Id == item.Id);
                if (toDoItem != null)
                {
                    toDoItem.Title = item.Title;
                    toDoItem.IsCompleted = item.IsCompleted;
                }
                _context.SaveChanges();
            }
            else
                throw new ArgumentNullException(nameof(item));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void Delete(Guid id)
    {
        try
        {
            var item = _context.ToDo.FirstOrDefault(ToDo => ToDo.Id == id);
            if (item != null)
            {
                _context.ToDo.Remove(item);
            }
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    
}
