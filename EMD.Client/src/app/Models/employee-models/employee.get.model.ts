export interface EmployeeGetModel {
  employeeId: number;
  employeeName: string;
  phoneNo: string;
  email: string;
  city: string;
  state: string;
  pincode: string;
  address: string;
  altPhoneNo?: string | null;
  designation: string;
  department: string;
  role: string;
}
