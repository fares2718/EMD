import { Component, DestroyRef, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { LoginRequest } from '../../Models/auth-models/Login.request';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private destroyRef = inject(DestroyRef);
  private authService = inject(AuthService);
  private router = inject(Router);

  errorMessage = signal<string | null>(null);

  form = new FormGroup({
    email: new FormControl('', {
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      validators: [Validators.required, Validators.minLength(6)],
    }),
  });

  get isEmailInvalid() {
    const c = this.form.controls.email;
    return c.touched && c.dirty && c.invalid;
  }

  get isPasswordInvalid() {
    const c = this.form.controls.password;
    return c.touched && c.dirty && c.invalid;
  }

  onSubmit() {
    if (this.form.invalid) return;

    const request: LoginRequest = {
      email: this.form.value.email ?? '',
      password: this.form.value.password ?? '',
    };

    this.authService
      .login(request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.errorMessage.set(null);
          const role = this.authService.currentUser()?.role;

          if (role === 'Admin') {
            this.router.navigate(['/dashboard/admin']);
          } else if (role === 'Employee') {
            this.router.navigate(['/dashboard/employee']);
          } else {
            this.router.navigate(['/login']);
          }
        },
        error: (err) => {
          this.errorMessage.set(err.message);
        },
      });
  }
}
