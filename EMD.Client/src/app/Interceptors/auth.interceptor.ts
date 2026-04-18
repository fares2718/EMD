import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  //const token = authService.getToken();

  // Avoid intercepting refresh token request itself
  if (req.url.includes('/refresh')) {
    return next(req);
  }

  let cloned = req;

  return next(cloned).pipe(
    catchError(err => {
      if (err.status === 401) {
        return authService.refreshToken().pipe(
          switchMap(res => {
            const retryReq = cloned.clone();
            return next(retryReq);
          }),
          catchError(() => {
            authService.logout();
            return throwError(() => err);
          })
        );
      }

      return throwError(() => err);
    })
  );
};
