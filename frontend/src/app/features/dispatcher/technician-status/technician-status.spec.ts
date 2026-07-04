import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TechnicianStatus } from './technician-status';

describe('TechnicianStatus', () => {
  let component: TechnicianStatus;
  let fixture: ComponentFixture<TechnicianStatus>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TechnicianStatus],
    }).compileComponents();

    fixture = TestBed.createComponent(TechnicianStatus);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
