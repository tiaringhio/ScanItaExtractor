import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputFormComponent } from '../input-form/input-form.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    InputFormComponent
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {

}
