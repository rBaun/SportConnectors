import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { loginService } from 'src/app/services/loginService';
import { NgForm } from "@angular/forms";
import { Router } from "@angular/router";

@Component({
  selector: 'app-front-page-image',
  templateUrl: './front-page-image.component.html',
  styleUrls: ['./front-page-image.component.css'],
  providers: []
})
export class FrontPageImageComponent implements OnInit {
  modalRef: BsModalRef | null;
  modalRef2: BsModalRef | null;
  modalRefRecoverPassword: BsModalRef;
  wrongEmailOrPassword : boolean = false;
  recoverPasswordResult : string = "Email has been sent";
clubRegistrationModal: BsModalRef;
  playerRegistrationModal: BsModalRef;
  constructor(private modalService: BsModalService, private loginService: loginService,
    private router: Router) { }
 

  
  

  
  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, {class: 'customModal'});
    this.wrongEmailOrPassword = false;
  }

  openSecondModalNested(template: TemplateRef<any>) {
    this.modalRef2 = this.modalService.show(template, {class: 'customModal'});
    this.modalRef.hide();
    this.modalRef = null;
  }

  openRecoverPasswordModal(template: TemplateRef<any>) {
    this.modalRefRecoverPassword = this.modalService.show(template, {class: 'customModal'});
  }
  
  recoverPasswordClicked(form : NgForm) {
    // this.loginService.revocerPassword(form.value.email).subscribe(
    //   (succes) =>(this.recoverPasswordResult = /*Tildel værdien fra succes beskden som kommer fra server*/""), 
    //   (error) => (this.recoverPasswordResult= "asd" + error /* TIldel recoverPasswordREsult værdien fra error beskden fra serveren*/),
    //   () => (console.log("Completed")))
  }

  closeAllModals() {
    this.modalRef.hide();
    this.modalRef = null;
    if(this.modalRefRecoverPassword != null) {
      this.modalRefRecoverPassword.hide();
      this.modalRefRecoverPassword = null;
    }
  }

  loginUser(form: NgForm) {
    
    this.loginService.loginUser(form).subscribe(
      (succes:any) => {      
        this.closeAllModals();
        console.log(succes);
        if(succes.isPlayer) {
          this.loginService.setupPlayerLogin(succes);
          this.router.navigate(['/player-dashboard'])
        }
        else if(succes.isClub) {
          this.loginService.setupClubLogin(succes);
          this.router.navigate(['/club-dashboard'])
        }

      },
      error => {
        if(error.error == "Failed to authenticate") {
          this.wrongEmailOrPassword = true;
        }
      }
    )
    form.resetForm();
  }

  openPlayerRegistrationModal(template: TemplateRef<any>) {
    this.playerRegistrationModal = this.modalService.show(template);
    this.modalRef.hide();
    this.modalRef = null;
  }

  openClubRegistrationModal(template: TemplateRef<any>) {
    this.clubRegistrationModal = this.modalService.show(template);
    this.modalRef.hide();
    this.modalRef = null;
  }


  ngOnInit() {
  }
    
  }
