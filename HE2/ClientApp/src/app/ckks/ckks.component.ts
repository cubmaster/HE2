import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-ckks',
  templateUrl: './ckks.component.html',
  styleUrls: ['./ckks.component.css']
})
export class CkksComponent implements OnInit {

  constructor(private http: HttpClient,  @Inject('BASE_URL') private baseUrl: string) {

  }

  ngOnInit() {
  }
  test(){

    this.http.get(this.baseUrl + 'CKKS/test').subscribe((result:any) => {
      window.alert(result);

    }, error => console.error(error));
  }
}
