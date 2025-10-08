import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ClipboardService {

  constructor() {}

    public async copyToClipBoard(text: string): Promise<void>{

      try{
        await navigator.clipboard.writeText(text);
      }catch(err){
        console.error('Erro ao copiar texto', err);
        throw new Error('Falha ao copiar.');
      }
    }
}
