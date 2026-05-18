import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { DepartmentService } from '../../Services/department.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-department',
  imports: [],
  templateUrl: './department.component.html',
  styleUrl: './department.component.css',
})
export class DepartmentComponent implements OnInit {
private readonly departmentService = inject(DepartmentService);
private readonly destroyRef = inject(DestroyRef);

departments = this.departmentService.loadedDepartments;

ngOnInit() {
  this.departmentService.getAllDepartments()
  .pipe(takeUntilDestroyed(this.destroyRef))
  .subscribe({
    next: (list) => {
      this.departments = signal(list);
    },
    error: (err) => {
      console.error('Error loading departments:', err);
    }
  })
}

}
