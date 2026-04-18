import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, tap, throwError } from 'rxjs';
import { EmployeeGetModel } from '../Models/employee-models/employee.get.model';
import { EmployeeModel } from '../Models/employee-models/employee.model';


@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  private http = inject(HttpClient);
  //private errorService = inject(ErrorService);

  private apiUrl = 'https://localhost:7082/api/Employee';

  private employees = signal<EmployeeGetModel[]>([]);
  loadedEmployees = this.employees.asReadonly();

  loadAllEmployees() {
    return this.fetchEmployees(`${this.apiUrl}/Filter`).pipe(
      tap({
        next: (list) => this.employees.set(list),
      })
    );
  }

  loadEmployeeById(id: number) {
    return this.http.get<EmployeeGetModel>(`${this.apiUrl}/${id}`,{withCredentials:true}).pipe(
      catchError(() => {
        //this.errorService.showError('Could not load employee');
        return throwError(() => new Error('Could not load employee'));
      })
    );
  }

  filterEmployees(filters: any) {
    let queryParams = new HttpParams();

    Object.keys(filters).forEach(key => {
      if (filters[key] && filters[key] !== '' && filters[key] !==  undefined ) {
        queryParams = queryParams.set(key, filters[key]);
      }
    });

    return this.http.get<EmployeeGetModel[]>
    (`${this.apiUrl}/Filter`, { params: queryParams ,withCredentials:true})
    .pipe(
      catchError(() => {
        //this.errorService.showError('Could not apply filters');
        return throwError(() => new Error('Could not apply filters'));
      })
    );

  }

  addEmployee(model: EmployeeModel){
    return this.http.post(`${this.apiUrl}/Add`, model,{withCredentials:true}).pipe(
      catchError(() => {
        //this.errorService.showError('Could not add employee');
        return throwError(() => new Error('Could not add employee'));
      })
    );
  }

  getTotalEmployeesCount() {
    return this.http.get<number>(`${this.apiUrl}/Count`).pipe(
      catchError(() => {
        //this.errorService.showError('Could not load employee count');
        return throwError(() => new Error('Could not load employee count'));
      })
    );
  }

  deleteEmployee(id: number) {
    return this.http.delete(`${this.apiUrl}/Delete/${id}`,{withCredentials:true}).pipe(
      catchError(() => {
        //this.errorService.showError('Could not delete employee'); 
        return throwError(() => new Error('Could not delete employee'));
      } )
    );
  }


  private fetchEmployees(url: string) {
    return this.http.get<EmployeeGetModel[]>(url,{withCredentials:true}).pipe(
      catchError(() => {
        //this.errorService.showError('Something went wrong');
        return throwError(() => new Error('Something went wrong'));
      })
    );
  }
}
