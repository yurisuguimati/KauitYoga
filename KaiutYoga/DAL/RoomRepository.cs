using KaiutYoga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.DAL
{
    public class RoomRepository
    {
        private KaiutYogaContext _context;

        public RoomRepository (KaiutYogaContext context)
        {
            this._context = context;
        }

        public IEnumerable<RoomModel> GetRooms()
        {
            return _context.RoomModels.ToList();
        }

        public RoomModel GetRoom(long roomId)
        {
            return _context.RoomModels.Where(m => m.Id == roomId).First();
        }
    }
}