using System;
using PassLocker.Database;

namespace PassLocker.Services
{
    public interface IProtector
    {
        public User CreateHashedPassword(User user);
        public bool CheckPassword(User user);
    }
}
