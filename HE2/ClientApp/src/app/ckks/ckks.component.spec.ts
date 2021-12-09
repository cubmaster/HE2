import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CkksComponent } from './ckks.component';

describe('CkksComponent', () => {
  let component: CkksComponent;
  let fixture: ComponentFixture<CkksComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CkksComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CkksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
