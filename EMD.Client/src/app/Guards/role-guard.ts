import { inject, Injectable } from "@angular/core";
import { CanActivate, ActivatedRouteSnapshot, Router } from "@angular/router";
import { AuthService } from "../Services/auth.service";

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {

  private authService = inject(AuthService);
  private router = inject(Router);

  canActivate(route: ActivatedRouteSnapshot): boolean {

    const userRole = this.authService.currentUser()?.role;
    const requiredRole = route.data['role'];

    if (!userRole) {
      this.router.navigate(['login']);
      return false;
    }

    if (userRole === requiredRole) {
      return true;
    }

    this.router.navigate(['dashboard']);
    return false;
  }
}
