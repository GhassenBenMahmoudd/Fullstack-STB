import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeclarationCorruptionComponent } from './declaration-corruption.component';

describe('DeclarationCorruptionComponent', () => {
  let component: DeclarationCorruptionComponent;
  let fixture: ComponentFixture<DeclarationCorruptionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeclarationCorruptionComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DeclarationCorruptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
