import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { DepartmentGetModel } from '../Models/department-models/department.get.model';

@Injectable({
  providedIn: 'root',
})
export class DepartmentService {
    private http = inject(HttpClient);
  //private errorService = inject(ErrorService);

  private departments = signal<DepartmentGetModel[]>([]);
  loadedDepartments = this.departments.asReadonly();

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

getAllDepartments(){
  return this.http.get<DepartmentGetModel[]>(`${this.apiUrl}/GetAllDepartments`,
    {withCredentials:true})
    .pipe( 
      catchError(() => {
        //this.errorService.showError('Could not load departments');
        return throwError(() => new Error('Could not load departments'));
      })
    );
}

}
