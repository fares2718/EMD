using AutoMapper;
using BCrypt.Net;
using EMD.BLL.DTOs;
using EMD.DAL.DA;
using EMD.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMD.BLL
{
    public class SessionBusiness
    {
        private readonly SessionsDataAccess _sessionsDataAccess;
        private readonly IMapper _mapper;

        public SessionBusiness(SessionsDataAccess sessionsDataAccess, IMapper mapper)
        {
            _sessionsDataAccess = sessionsDataAccess;
            _mapper = mapper;
        }

        public async Task<int> AddSessionAsync(int UserId, string RefreshToken, DateTime ExpirationDate)
        {
            var session = new Session
            {
                UserId = UserId,
                RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(RefreshToken),
                RefreshTokenExpiresAt = ExpirationDate,
                RefreshTokenRevokedAt = null
            };
            return await _sessionsDataAccess.AddSessionAsync(session);
        }

        public async Task<SesstionDTO?> GetSessionByUserIdAsync(int userId,string refreshToken)
        {
            var sessions = await _sessionsDataAccess.GetSessionByUserIdAsync(userId);
            if (sessions.Count == 0)
            {
                return null;
            }
            var session = sessions.SingleOrDefault(s => BCrypt.Net.BCrypt.Verify(refreshToken,s.RefreshTokenHash));
            if (session is null)
                return null;
            return _mapper.Map<SesstionDTO>(session);
        }

        public async Task<SesstionDTO?> GetSessionByIdAsync(int sessionId)
        {
            var session = await _sessionsDataAccess.GetSessionByIdAsync(sessionId);
            var dto = _mapper.Map<SesstionDTO>(session);
            return dto;
        }

        public async Task<bool> RevokeSessionAsync(int sessionId)
        {
            var session = await _sessionsDataAccess.GetSessionByIdAsync(sessionId);
            if (session != null)
            {
                session.RefreshTokenRevokedAt = DateTime.UtcNow;
                return await _sessionsDataAccess.UpdateSessionAsync(session);
            }
            return false;
        }

        public async Task<bool> UpdateSessionAsync(int sessionId, string newRefreshToken, DateTime newExpirationDate)
        {
            var session = new Session
            {
                SessionId = sessionId,
                RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken),
                RefreshTokenExpiresAt = newExpirationDate,
                RefreshTokenRevokedAt = null
            };
            return await _sessionsDataAccess.UpdateSessionAsync(session);
        }
    }
}
