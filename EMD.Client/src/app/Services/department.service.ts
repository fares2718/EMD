import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DepartmentService {
    private http = inject(HttpClient);
  //private errorService = inject(ErrorService);

  private apiUrl = 'https://localhost:7082/api/Department';
getDepartmentsCount(): Observable<number> {
  return this.http.get<number>(`${this.apiUrl}/ActiveCount`,{withCredentials:true})
  .pipe(
    catchError(() => {
      //this.errorService.showError('Could not load departments count');
      return throwError(() => new Error('Could not load departments count'));
    })
  );
}

}
