import { HttpClient } from "@angular/common/http";
import { inject, Injectable, signal } from "@angular/core";
import { BehaviorSubject, catchError, switchMap, tap, throwError } from "rxjs";
import { LoginRequest } from "../Models/auth-models/Login.request";
import { TokenResponse } from "../Models/auth-models/token.response";
import { jwtDecode } from "jwt-decode";
import { EmployeeService } from "../Services/employee.service";
import { EmployeeModel } from "../Models/employee-models/employee.model";
import { EmployeeGetModel } from "../Models/employee-models/employee.get.model";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private employeeService = inject(EmployeeService);

  private apiUrl = "https://localhost:7082/api/Auth";

  currentUser = signal<EmployeeGetModel|null>(null);

  private authState = new BehaviorSubject<boolean>(false);
  authState$ = this.authState.asObservable();

login(model: LoginRequest) {
  return this.http.post<EmployeeGetModel>(`${this.apiUrl}/login`, model, {
    withCredentials: true
  }).pipe(

    tap(res => {
      this.currentUser.set(res);
      this.authState.next(true);
    }),
    catchError(err => {
      this.authState.next(false);

      let errorMessage = 'Something went wrong';

      if (err.status === 401) {
        errorMessage = 'Invalid email or password';
      } else if (err.status === 0) {
        errorMessage = 'Server is unreachable';
      }

      return throwError(() => new Error(errorMessage));
    })
  );
}

  refreshToken() {
    return this.http.post<EmployeeGetModel>(`${this.apiUrl}/refresh`, {}, {
      withCredentials: true
    }).pipe(
      tap(res => {
        this.authState.next(true);
        this.currentUser.set(res);
      }),
    );
  }

  logout() {
    this.authState.next(false);

    return this.http.post(`${this.apiUrl}/logout`, {}, {
      withCredentials: true
    });
  }

  isAuthenticated(): boolean {
    return this.authState.value;
  }
}