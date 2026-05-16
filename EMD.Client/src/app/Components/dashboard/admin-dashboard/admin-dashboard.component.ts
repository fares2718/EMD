import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '../../../Services/auth.service';
import { Router } from '@angular/router';
import { EmployeeService } from '../../../Services/employee.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DepartmentService } from '../../../Services/department.service';
import { DesignationService } from '../../../Services/designation.service';

@Component({
  selector: 'app-admin-dashboard',
  imports: [],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css',
})
export class AdminDashboardComponent implements OnInit {
private authService = inject(AuthService);
private destroyRef = inject(DestroyRef);
private employeeService = inject(EmployeeService);
private departmentService = inject( DepartmentService);
private designationService = inject(DesignationService);
private router = inject(Router);
currentUser =this.authService.currentUser;
totalEmployees = signal<number>(0);
totalDepartments = signal<number>(0);
totalDesignations = signal<number>(0);



ngOnInit(): void {
  this.employeeService.getTotalEmployeesCount()
  .pipe(takeUntilDestroyed(this.destroyRef))
  .subscribe({
    next: (count) => this.totalEmployees.set(count)
  });

  this.departmentService.getDepartmentsCount()
  .pipe(takeUntilDestroyed(this.destroyRef))
  .subscribe({
    next: (count) => this.totalDepartments.set(count)
  });

  this.designationService.getDesignationsCount()
  .pipe(takeUntilDestroyed(this.destroyRef))
  .subscribe({
    next: (count) => this.totalDesignations.set(count)
  });
}

onAddEmployeeClicked(){
  this.router.navigate(['/new-employee']);
}

onViewEmployeesClicked(){
  this.router.navigate(['/employees']);
}

onManageDepartmentsClicked(){
  this.router.navigate(['/departments']);
}

onManageDesignationsClicked(){
this.router.navigate(['/designations']);
}
}
