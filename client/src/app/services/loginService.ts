//import { nameModel } from "../models/name.model";
import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http';

@Injectable()

export class loginService {
  constructor(private http: HttpClient) {}

  revocerPassword(email: string) {
    let url = "WEB API controller metode";
    return this.http.post(url, email);
  }

  clubLogin(email: string, password: string) {
    let url = "WEB API controller metode";
    return this.http.post(url,{
      email: email,
      password : password
    });
  }

  playerLogin(email: string, password: string) {
    let url = "WEB API controller metode";
    return this.http.post(url,{
      email: email,
      password : password
    });
  }

}