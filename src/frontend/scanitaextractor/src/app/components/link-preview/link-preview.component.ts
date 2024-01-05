import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PreviewLink } from 'src/app/helpers/models/preview-link.model';
import { PdfgeneratorService } from 'src/app/services/pdfgenerator.service';

@Component({
  selector: 'app-link-preview',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './link-preview.component.html',
  styleUrls: ['./link-preview.component.scss']
})
export class LinkPreviewComponent {

  preview: PreviewLink | null = null;

  constructor(public pdfGenerator: PdfgeneratorService) {
    this.pdfGenerator.previewLink$.subscribe((result) => {
      this.preview = result;
    })
  }
}
