using CoreBase.Authentication;
using CoreBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBase.Data
{
    public class CommanderRepo : ICommanderRepo
    {
        #region Declare Variables
        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor
        public CommanderRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Get all commands
        public IEnumerable<Command> GetAllCommands()
        {
            return _context.Commands.ToList();
        }
        #endregion

        #region Get command by id

        public Command GetCommandById(int id)
        {
            return _context.Commands.FirstOrDefault(p=>p.Id == id);
        }
        #endregion

        #region Create Command

        public void CreateCommand(Command cmd)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));

            _context.Commands.Add(cmd);
        }
        #endregion

        #region Update Command
        public void UpdateCommand(Command cmd)
        {
        }
        #endregion

        #region Delete Command

        public void DeleteCommand(Command cmd)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));

            _context.Commands.Remove(cmd);
        }
        #endregion

        #region Save db
        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }


        #endregion
    }
}
