import { Component, DestroyRef, inject } from '@angular/core';
import { AuthService } from '../../Services/auth.service';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-header',
  imports: [],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
private authService = inject(AuthService);
private router = inject(Router);
private destroyRef = inject(DestroyRef);
logOut(){
  this.authService.logout()
  .pipe(takeUntilDestroyed(this.destroyRef))
  .subscribe(
    {
      next: () => {this.router.navigate(['/login']);},
    }
  );
}

}
