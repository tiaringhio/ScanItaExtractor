import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ScanItaUrlPattern } from 'src/app/helpers/utils/patterns';
import { PdfgeneratorService } from 'src/app/services/pdfgenerator.service';

@Component({
  selector: 'app-input-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './input-form.component.html',
  styleUrls: ['./input-form.component.scss']
})
export class InputFormComponent {

 constructor(public pdfGenerator: PdfgeneratorService) {}

  formGroup = new FormGroup({
    scanUrl: new FormControl('', [
      Validators.required,
      Validators.pattern(ScanItaUrlPattern)
    ]),
  })


  get m() {
    return this.formGroup.controls;
  }

  submit() {
    if (this.formGroup.valid) {
      this.pdfGenerator.generatePdf(this.formGroup.value.scanUrl as string)
      .subscribe((result) => {
        const downloadURL = window.URL.createObjectURL(result);
        const link = document.createElement('a');
        window.open(URL.createObjectURL(result));
        link.href = downloadURL;
        link.download = this.pdfGenerator.pdfname;
        link.click();
        })
    }
    else {
      this.formGroup.markAllAsTouched();
    }
  }
}
