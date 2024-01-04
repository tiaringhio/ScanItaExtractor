import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputFormComponent } from '../input-form/input-form.component';
import { Meta } from "@angular/platform-browser";

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

  constructor(private _meta: Meta) {
    this._meta.addTag({ name: 'title', content: 'Scan Ita PDF Extractor' });
    this._meta.addTag({ name: 'og:title', content: 'Scan Ita PDF Extractor' });
    this._meta.addTag({ name: 'og:description', content: 'Transform you Scan Ita experience by creating high resolution PDFs for your favorite mangas!' });
  }
}
