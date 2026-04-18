import { inject, Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { AuthService } from "../Services/auth.service";
import { catchError, map, Observable, of } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate{
private authService = inject(AuthService)
private router = inject (Router)
  
  canActivate():Observable<boolean> {
    if(this.authService.isAuthenticated()){
      return of(true);
    }else{
      return this.authService.refreshToken().pipe(
      map(() => true),
      catchError(() => {
        this.router.navigate(['login']);
        return of(false);
      })
    );
    }
  }

}
