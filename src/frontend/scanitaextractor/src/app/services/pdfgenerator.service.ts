import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, EMPTY, Subject, catchError, map } from 'rxjs';
import { PreviewLink } from '../helpers/models/preview-link.model';

@Injectable({
  providedIn: 'root'
})
export class PdfgeneratorService {

  public generatedPdf$: Subject<Blob>
  public isLoading$: Subject<boolean>
  private _pdfname: BehaviorSubject<string>;
  public previewLink$: BehaviorSubject<PreviewLink | null>;
  isLivePreviewEnabled$: BehaviorSubject<boolean>;

  constructor(private _http: HttpClient) {
    this.generatedPdf$ = new Subject<Blob>();
    this.isLoading$ = new Subject<boolean>();
    this._pdfname = new BehaviorSubject<string>("");
    this.previewLink$ = new BehaviorSubject<PreviewLink | null>(null);
    this.isLivePreviewEnabled$ = new BehaviorSubject<boolean>(true);
  }

  get pdfname(): string {
    return this._pdfname.getValue();
  }

  generatePdf(scanUrl: string) {
    this.isLoading$.next(true);
    return this._http.get(`${environment.apiUrl}/api/Generator/GeneratePdf`, {
      responseType: 'blob',
      observe: 'response',
      params: {
        scanUrl: scanUrl
      }
    }).pipe(map((result) => {
      let contentDisposition = result.headers.get('Content-Disposition');
      let filename = contentDisposition?.split(';')[1].split('=')[1].replace(/"/g, '');
      this._pdfname.next(filename ?? "scanita.pdf");
      this.isLoading$.next(false);
      this.generatedPdf$.next(result.body as Blob);
      return result.body as Blob;
    }), catchError((err) => {
      this.isLoading$.next(false);
      return EMPTY;
    }));
  }

  previewLink(scanUrl: string) {
    return this._http.get<PreviewLink>(`${environment.apiUrl}/api/Generator/PreviewLink`,
    {
      params: {
        link: scanUrl
      }
    })
      .pipe(map((result) => {
        const [title] = result.title.split(' - ');
        const preview = { ...result, title };
        this.previewLink$.next(preview);
        return result;
      }), catchError((err) => {
        if (err.status === 429) {
          this.isLivePreviewEnabled$.next(false);
        }
        return EMPTY;
      }));
  }
}
