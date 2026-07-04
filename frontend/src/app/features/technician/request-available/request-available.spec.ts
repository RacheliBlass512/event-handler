import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestAvailable } from './request-available';

describe('RequestAvailable', () => {
  let component: RequestAvailable;
  let fixture: ComponentFixture<RequestAvailable>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RequestAvailable],
    }).compileComponents();

    fixture = TestBed.createComponent(RequestAvailable);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
