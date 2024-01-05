import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ScanItaUrlPattern } from 'src/app/helpers/utils/patterns';
import { PdfgeneratorService } from 'src/app/services/pdfgenerator.service';
import { LinkPreviewComponent } from '../link-preview/link-preview.component';
import { debounce, pairwise, startWith, timer } from 'rxjs';

@Component({
  selector: 'app-input-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LinkPreviewComponent],
  templateUrl: './input-form.component.html',
  styleUrls: ['./input-form.component.scss']
})
export class InputFormComponent implements OnInit {

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

  ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(
        startWith(this.formGroup.value),
        pairwise(),
        debounce(([prev,next]) => prev.scanUrl !== next.scanUrl ? timer(500) : timer(0))
      )
      .subscribe(([prev, next]) => {
        this.getPreviewLink(next.scanUrl as string);
    })
  }

  getPreviewLink(scanUrl: string) {
    if (this.formGroup.valid) {
      this.pdfGenerator.previewLink(scanUrl).subscribe(() => {})
    }
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
