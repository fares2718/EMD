import { Component, inject } from '@angular/core';
import { AuthService } from '../../Services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  imports: [],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
private authService = inject(AuthService);
private router = inject(Router);
logOut(){
  this.authService.logout().subscribe(
    {
      next: () => {this.router.navigate(['/login']);},
    }
  );
}

}
