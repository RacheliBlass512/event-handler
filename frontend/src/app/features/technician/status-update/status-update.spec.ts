import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatusUpdate } from './status-update';

describe('StatusUpdate', () => {
  let component: StatusUpdate;
  let fixture: ComponentFixture<StatusUpdate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusUpdate],
    }).compileComponents();

    fixture = TestBed.createComponent(StatusUpdate);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
