import { Routes } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { DashboardComponent } from './Components/dashboard/dashboard.component';
import { EmployeeForm } from './Components/employee/employee-form/employee-form.component';
import { EmployeeListComponent } from './Components/employee/employee-list/employee-list.component';
import { Department } from './Components/department/department';
import { Designation } from './Components/designation/designation';
import { AuthGuard } from './Guards/auth-guard';
import { MainLayoutComponent } from './Components/main-layout/main-layout.component';
import { AdminDashboardComponent } from './Components/dashboard/admin-dashboard/admin-dashboard.component';
import { EmployeeDashboardComponent } from './Components/dashboard/employee-dashboard/employee-dashboard.component';
import { RoleGuard } from './Guards/role-guard';
import { EmployeeCard } from './Components/employee/employee-card/employee-card';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
    },
    {
        path: 'login',
        component: LoginComponent,
    },
    {
        path:'',
        component: MainLayoutComponent,
        canActivate: [AuthGuard],
        children:[
            {
                path:'dashboard',
                component: DashboardComponent,
                children:[
                    {
                        path:'admin',
                        component: AdminDashboardComponent,
                        canActivate: [RoleGuard],
                        data: { role: 'Admin' }
                    },
                    {
                        path:'employee',
                        component: EmployeeDashboardComponent,
                        canActivate: [RoleGuard],
                        data: { role: 'Employee' }
                    }
                ]
            },
            {
                path:'new-employee',
                component: EmployeeForm
            },
            {
                path:'employees',
                component:EmployeeListComponent,
            },
            {
                path:'employee/:id',
                component: EmployeeCard
            },
            {
                path:'department',
                component:Department
            },
            {
                path:'designation',
                component:Designation
            }
        ]
    }
];
