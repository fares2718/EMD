import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { EmployeeService } from '../../../Services/employee.service';
import { EmployeeGetModel } from '../../../Models/employee-models/employee.get.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-employee-list',
  imports: [ReactiveFormsModule],
  templateUrl: './employee-list.component.html',
  styleUrl: './employee-list.component.css',
})
export class EmployeeListComponent implements OnInit {
  filtersForm = new FormGroup({
    name: new FormControl('', {
      validators: [],
    }),

    email: new FormControl('', {
      validators: [],
    }),

    phone: new FormControl('', {
      validators: [],
    }),

    city: new FormControl('', {
      validators: [],
    }),

    state: new FormControl('', {
      validators: [],
    }),

    designationId: new FormControl('', {
      validators: [],
    }),

    sortBy: new FormControl('name', {
      validators: [],
    }),

    isDescending: new FormControl(false, {
      validators: [],
    }),

    pageNumber: new FormControl(1, {
      validators: [],
    }),

    pageSize: new FormControl(10, {
      validators: [],
    }),
  });

  private employeeService = inject(EmployeeService);
  private destroyRef = inject(DestroyRef);

  employees = signal<EmployeeGetModel[]>([]);

  ngOnInit() {
    this.employeeService
      .loadAllEmployees()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (list) => this.employees.set(list),
        error: (err) => console.error(err),
      });
  }

  applyFilters() {
    const filters = this.filtersForm.value;
    this.employeeService
      .filterEmployees(filters)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (list) => this.employees.set(list),
        error: (err) => console.error(err),
      });
  }

  nextPage() {
    const current = this.filtersForm.value.pageNumber;
    if (!current) return;
    this.filtersForm.patchValue({ pageNumber: current + 1 });
    this.applyFilters();
  }

  previousPage() {
    const current = this.filtersForm.value.pageNumber;
    if (!current) return;
    if (current > 1) {
      this.filtersForm.patchValue({ pageNumber: current - 1 });
      this.applyFilters();
    }
  }

  resetFilters() {
    this.filtersForm.reset({
      name: '',
      email: '',
      phone: '',
      city: '',
      state: '',
      designationId: '',
      sortBy: 'name',
      isDescending: false,
      pageNumber: 1,
      pageSize: 10,
    });
  }

  onDeleteBtnClicked(Id:number){
    this.employeeService.deleteEmployee(Id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        console.log('Employee deleted successfully');
        this.applyFilters();
      },
      error: (err) => console.error(err)
    });
  }
}
