import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";



@Component({
  selector: 'app-he-encrypt',
  templateUrl: './he-encrypt.component.html',
  styleUrls: ['./he-encrypt.component.css']
})
export class HeEncryptComponent implements OnInit {
  result: Artifact;
  testValue: Number;
  status: String;
  constructor(private http: HttpClient,  @Inject('BASE_URL') private baseUrl: string) {

  }

  ngOnInit() {
    this.status="Waiting...";
  }
  encrypt(){
    this.status="Encrypting...";
    this.http.post(this.baseUrl + 'BFV/Encrypt',this.testValue).subscribe((result:Artifact) => {
      this.result = result;
      console.log(this.result);
      this.status="encrypted";
    }, error => console.error(error));
  }

  execute() {

    this.status="calculating...";
    this.http.post(this.baseUrl + 'BFV/Execute',this.result).subscribe((val:any) => {

      var fact = new Artifact();
      fact.key = this.result.key;
      fact.data = val.data;
      this.status="decrypting...";
      this.decrypt(fact);

    });
  }

  decrypt(fact:any) {
    this.http.post(this.baseUrl + 'BFV/Decrypt', fact).subscribe((result) => {


      window.alert(result);
      this.status="waiting...";
    });
  }
  test(){

    this.http.post(this.baseUrl + 'BFV/test',this.testValue).subscribe((result:any) => {
      window.alert(result);

    }, error => console.error(error));
  }

}

export class Artifact{
  public settings: string;
  public data: string;
  public key: string;
}
