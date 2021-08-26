import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  //@Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();


  model: any = {};
  registerForm: FormGroup;

  constructor(private accountSerice: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.registerForm = new FormGroup({
      username: new FormControl('',Validators.required),
      password: new FormControl('',[Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
    })

    this.registerForm.controls.password.valueChanges.subscribe(() => {
        this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })

  }

  matchValues(matchTo:string): ValidatorFn {
    return (control: AbstractControl): {[key: string]: any} | null => {
      const forbidden = control?.parent?.controls as any;
      return (forbidden) 
        ? (control?.value === forbidden[matchTo]?.value) ? null : {isMatching: true}
        : null;
    }
  }
  



  register(){
    console.log(this.registerForm.value);
    // this.accountSerice.register(this.model).subscribe(res => {
    //   console.log(res);
    //   this.cancel();
    // }, error => {
    //   console.log(error);
    //   this.toastr.error(error.error);
    // })
    
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

  

}
