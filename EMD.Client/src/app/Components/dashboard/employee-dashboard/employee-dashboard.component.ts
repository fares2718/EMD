import { Component, inject } from '@angular/core';
import { AuthService } from '../../../Services/auth.service';

@Component({
  selector: 'app-employee-dashboard',
  imports: [],
  templateUrl: './employee-dashboard.component.html',
  styleUrl: './employee-dashboard.component.css',
})
export class EmployeeDashboardComponent {
private authService = inject(AuthService);

currentUser =this.authService.currentUser;
}
