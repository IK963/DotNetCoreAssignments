using DotNetCoreAssignments.Models;

public class ToDoRepository
{
    #region members

    private readonly List<ToDo> _todoList = new List<ToDo>();

    #endregion

    public IEnumerable<ToDo> GetAll()
    {
        return _todoList;
    }

    public ToDo GetById(int id)
    {
        return _todoList.FirstOrDefault(ToDo => ToDo.Id == id);
    }

    public ToDo Create(ToDo item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(ToDo));
        }

        int maxItem = _todoList != null && _todoList.Count > 0 ? _todoList.Max(x => x.Id) + 1 : 1;
        item.Id = maxItem;

        if (_todoList != null)
            _todoList.Add(item);
        return item;
    }

    public void Update(ToDo item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        var toDoItem = _todoList.FirstOrDefault(ToDo => ToDo.Id == item.Id);
        if (toDoItem != null)
        {
            toDoItem.Title = item.Title;
            toDoItem.IsCompleted = item.IsCompleted;
        }
    }

    public void Delete(int id)
    {
        var item = _todoList.FirstOrDefault(ToDo => ToDo.Id == id);
        if (item != null)
        {
            _todoList.Remove(item);
        }
    }
}
