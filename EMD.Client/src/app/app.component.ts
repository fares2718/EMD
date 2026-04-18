import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { AuthService } from './Services/auth.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { switchMap } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  protected readonly title = signal('EDM');
  private authService = inject(AuthService);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);

  ngOnInit() {
    this.authService.refreshToken()
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (res) => {;
        const role = this.authService.currentUser()?.role;

        if (role === 'Admin') {
          this.router.navigate(['/dashboard/admin']);
        } else if (role === 'Employee') {
          this.router.navigate(['/dashboard/employee']);
        } else {
          this.router.navigate(['/login']);
        }
      },
      error: () => console.log('Not logged in'),
    });
  }
}
