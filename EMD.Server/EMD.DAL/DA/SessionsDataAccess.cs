using EMD.EF;
using EMD.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMD.DAL.DA
{
    public class SessionsDataAccess
    {
        private readonly EMDDbContext _context;

        public SessionsDataAccess(EMDDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddSessionAsync(Session session)
        {
            await _context.Sessions.AddAsync(session);
            await _context.SaveChangesAsync();
            return session.SessionId;
        }

        public async Task<List<Session>> GetSessionByUserIdAsync(int userId)
        {
            var sessions = await _context.Sessions.Where(s => s.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

            return sessions;
        }

        public async Task<Session?> GetSessionByIdAsync(int sessionId)
        {
            return await _context.Sessions
            .AsNoTracking()
            .FindAsync(sessionId);
        }

        public async Task<bool> UpdateSessionAsync(Session session)
        {
            var existingSession = await _context.Sessions.FindAsync(session.SessionId);
            if (existingSession!= null)
            {
                existingSession.RefreshTokenHash = session.RefreshTokenHash;
                existingSession.RefreshTokenExpiresAt = session.RefreshTokenExpiresAt;
                existingSession.RefreshTokenRevokedAt = session.RefreshTokenRevokedAt;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
