import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HeEncryptComponent } from './he-encrypt.component';

describe('HeEncryptComponent', () => {
  let component: HeEncryptComponent;
  let fixture: ComponentFixture<HeEncryptComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HeEncryptComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HeEncryptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
