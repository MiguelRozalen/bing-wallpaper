import { Component, OnInit } from '@angular/core';
import { Pipe } from '@angular/core';
import { catchError, retry } from 'rxjs/operators';
import { startWith } from 'rxjs/operators/startWith';
import { forkJoin } from "rxjs/observable/forkJoin";
import { ErrorObservable } from 'rxjs/observable/ErrorObservable';

import { HttpClient, HttpErrorResponse } from '@angular/common/http';

import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'my-app',
  templateUrl: 'app/app.component.html',
  styleUrls: ['app/app.component.scss'],
})
export class AppComponent implements OnInit {
  private url;
  constructor(private http: HttpClient, private sanitizer: DomSanitizer) {
    this.http.get("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-us").pipe(retry(3), catchError(this.handleError)).subscribe(response=>{
      console.log(response);
      this.url = "http://www.bing.com/"+response.images[0].url;
      //this.url = "https://www.bing.com/az/hprichbg/rb/DuskyDolphin_EN-US11918143365_1920x1080.jpg";
      document.getElementById('loading').style.opacity = "0";
    });
  }

  ngOnInit() {
  }

  sanitize(url) {
    return this.sanitizer.bypassSecurityTrustStyle('url(' + url+ ')');
  }

  handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
        // A client-side or network error occurred. Handle it accordingly.
        console.error('An error occurred:', error.error.message);
    } else {
        // The backend returned an unsuccessful response code.
        // The response body may contain clues as to what went wrong,
        console.error(`Backend returned code ${error.status}, body was: ${error.error}`);
    }
    // return an ErrorObservable with a user-facing error message
    return new ErrorObservable('Something bad happened; please try again later.');
  };
}