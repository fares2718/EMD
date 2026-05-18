import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { DesignationGetModel } from '../Models/designation-models/designation.get.model';

@Injectable({
  providedIn: 'root',
})
export class DesignationService {
    private http = inject(HttpClient);
  //private errorService = inject(ErrorService);

  private apiUrl = 'https://localhost:7082/api/Designation';

  getDesignationsCount(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/Count`,{withCredentials:true})
    .pipe(
      catchError(() => {
        //this.errorService.showError('Could not load designations count');
        return throwError(() => new Error('Could not load designations count'));
      })
    );
  }

  getDesignations(): Observable<DesignationGetModel[]> {
    return this.http.get<DesignationGetModel[]>(`${this.apiUrl}`, {withCredentials:true})
    .pipe(
      catchError(() => {
        //this.errorService.showError('Could not load designations');
        return throwError(() => new Error('Could not load designations'));
      })
    );
  }
}