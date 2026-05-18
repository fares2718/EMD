import { Component, inject, input } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { EmployeeModel } from '../../../Models/employee-models/employee.model';
import { EmployeeService } from '../../../Services/employee.service';
import { DesignationService } from '../../../Services/designation.service';
import { DesignationGetModel } from '../../../Models/designation-models/designation.get.model';

@Component({
  selector: 'app-employee-form',
  imports: [ReactiveFormsModule],
  templateUrl: './employee-form.component.html',
  styleUrl: './employee-form.component.css',
})
export class EmployeeForm {
private employeeService = inject(EmployeeService);
private designationService = inject(DesignationService);

form = new FormGroup({

    employeeName: new FormControl('', {
      validators: [
        Validators.required,
        Validators.maxLength(50),
        Validators.pattern(/^[\p{L}\s'-]+$/u)
      ]
    }),

    phoneNo: new FormControl('', {
      validators: [
        Validators.required,
        Validators.pattern(/^[0-9]{10}$/)
      ]
    }),

    email: new FormControl('', {
      validators: [
        Validators.required,
        Validators.email
      ]
    }),

    password: new FormControl('', {
      validators: [
        Validators.required,
        Validators.minLength(6)
      ]
    }),

    confirmPassword: new FormControl('', {
      validators: [
        Validators.required
      ]
    }),

    role: new FormControl('', {
      validators: [Validators.required]
    }),

    city: new FormControl('', {
      validators: [
        Validators.required,
        Validators.pattern(/^[\p{L}\s'-]+$/u)
      ]
    }),

    state: new FormControl('', {
      validators: [
        Validators.required,
        Validators.pattern(/^[\p{L}\s'-]+$/u)
      ]
    }),

    pincode: new FormControl('', {
      validators: [
        Validators.required,
        Validators.pattern(/^[0-9]{6}$/)
      ]
    }),

    address: new FormControl('', {
      validators: [Validators.required]
    }),

    altPhoneNo: new FormControl('', {
      validators: [
        Validators.pattern(/^[0-9]{10}$/)
      ]
    }),

    designation: new FormControl('', {
      validators: [Validators.required]
    })

  }, { validators: this.passwordMatchValidator });

  designations: DesignationGetModel[] = [];

  ngOnInit() {
    this.designationService.getDesignations().subscribe({
      next: (list) => {
        this.designations = list;
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

onSubmit(){
 const newEmployee: EmployeeModel = {
  employeeName: this.form.value.employeeName!,
  phoneNo: this.form.value.phoneNo!,
  email: this.form.value.email!,
  password: this.form.value.password!,
  role: this.form.value.role!,
  city: this.form.value.city!,
  state: this.form.value.state!,
  pincode: this.form.value.pincode!,
  address: this.form.value.address!,
  altPhoneNo: this.form.value.altPhoneNo ?? '',
  designationId: Number(this.form.value.designation!),
 }

 this.employeeService.addEmployee(newEmployee).subscribe({
  next: () => {
    this.form.reset();
  },
  error: (err) => {
    console.error(err);
    //this.toastrService.showError('Failed to add employee');
  }
 });

}

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirm = control.get('confirmPassword')?.value;

    if (!password || !confirm) return null;

    return password === confirm ? null : { passwordMismatch: true };
  }
}
