import { HttpErrorResponse, HttpHandler, HttpHeaders, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";

@Injectable()
export class APIInterceptor implements HttpInterceptor {

  constructor(private router: Router) { }

  intercept(request: HttpRequest<any>, next: HttpHandler) {


    let auth_token = localStorage.getItem('token');
    if (auth_token == null || auth_token == undefined) {
      return next.handle(request); 
    }
    var body = request.body;
    if (body instanceof FormData) {
      const formRequest = request.clone({
        headers: new HttpHeaders({
          'Accept': '*/*',
          //'Content-Type': 'multipart/form-data',
          'Authorization': "Bearer " + auth_token,
          'Access-Control-Allow-Methods': 'GET, POST, OPTIONS, PUT, PATCH, DELETE',
          'Access-Control-Allow-Headers': 'origin,X-Requested-With,content-type,accept',
          'Access-Control-Allow-Credentials': 'true'
        })
      })
      return next.handle(formRequest).pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            // Unauthorized - redirect to the login page
            localStorage.clear();
            this.router.navigate(['/login']);
          }
          return throwError(error);
        })
      );
    }
    const modifiedRequest = request.clone({
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': "Bearer " + auth_token
      })
    })
    return next.handle(modifiedRequest).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          // Unauthorized - redirect to the login page
          localStorage.clear();
          this.router.navigate(['user-authenticaton/login']);
        }
        return throwError(error);
      })
    );;
  }
}
