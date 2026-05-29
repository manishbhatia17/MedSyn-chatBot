import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({ name: 'linkify', standalone: true })
export class LinkifyPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}

  transform(value: string): SafeHtml {
    if (!value) return '';
    return this.sanitizer.bypassSecurityTrustHtml(this.render(value));
  }

  private render(text: string): string {
    const linkButtons: string[] = [];
    const tokens: string[] = [];

    // Handle markdown links [label](url) — use the label as the button text, collect for end
    text = text.replace(/\[([^\]]+)\]\((https?:\/\/[^\s)]+)\)/g, (_match, label, url) => {
      linkButtons.push(this.renderUrlWithLabel(url, label));
      return ''; // remove entirely from inline text; button appears at the end
    });

    // Handle bare https:// URLs — remove from inline, collect button for end
    text = text.replace(/(https?:\/\/[^\s<>"]+)/g, (url) => {
      linkButtons.push(this.renderUrl(url));
      return '';
    });

    // Handle email addresses — stay inline as mailto links
    text = text.replace(/([a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,})/g, (email) => {
      return `\x00EMAIL_${tokens.push(`<a href="mailto:${email}" class="chat-email-link">&#9993; ${email}</a>`) - 1}\x00`;
    });

    // Process line-by-line for bullet lists and newlines
    const lines = text.split('\n');
    const out: string[] = [];
    let inList = false;

    for (const line of lines) {
      const bullet = line.match(/^\s*[-*•]\s+(.+)$/);
      if (bullet) {
        if (!inList) { out.push('<ul class="chat-list">'); inList = true; }
        out.push(`<li>${this.inline(bullet[1])}</li>`);
      } else {
        if (inList) { out.push('</ul>'); inList = false; }
        const content = this.inline(line);
        out.push(content ? content + '<br>' : '<br>');
      }
    }
    if (inList) out.push('</ul>');

    let html = out.join('').replace(/(<br>\s*)+$/, '');

    // Restore inline tokens (labels and emails)
    html = html.replace(/\x00LABEL_(\d+)\x00/g, (_, i) => tokens[+i]);
    html = html.replace(/\x00EMAIL_(\d+)\x00/g, (_, i) => tokens[+i]);

    // Append all URL buttons at the end
    if (linkButtons.length > 0) {
      html += `<div class="chat-link-group">${linkButtons.join('')}</div>`;
    }

    return html;
  }

  private inline(text: string): string {
    const parts = text.split(/(\x00(?:LABEL|EMAIL)_\d+\x00)/);
    return parts.map((part, i) => {
      if (i % 2 === 1) return part; // token placeholder — pass through
      let s = this.escapeHtml(part);
      s = s.replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>');
      return s;
    }).join('');
  }

  private escapeHtml(text: string): string {
    return text
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;');
  }

  private renderUrlWithLabel(url: string, label: string): string {
    const href = url.includes('medgyn.com') ? 'https://www.medgyn.com' : url;
    return `<a href="${href}" target="_blank" rel="noopener noreferrer" class="chat-link-btn">${this.escapeHtml(label)}</a>`;
  }

  private renderUrl(url: string): string {
    if (url.includes('medgyn.com')) {
      return `<a href="https://www.medgyn.com" target="_blank" rel="noopener noreferrer" class="chat-link-btn">&#127760; Visit MedGyn</a>`;
    }
    if (url.includes('ups.com')) {
      return `<a href="${url}" target="_blank" rel="noopener noreferrer" class="chat-link-btn">&#128230; Track UPS Shipment</a>`;
    }
    if (url.includes('fedex.com')) {
      return `<a href="${url}" target="_blank" rel="noopener noreferrer" class="chat-link-btn">&#128230; Track FedEx Shipment</a>`;
    }
    if (url.toLowerCase().includes('invoice') || url.toLowerCase().endsWith('.pdf')) {
      return `<a href="${url}" target="_blank" rel="noopener noreferrer" class="chat-link-btn">&#128196; Download Invoice</a>`;
    }
    return `<a href="${url}" target="_blank" rel="noopener noreferrer" class="chat-link-btn">&#128279; Open Link</a>`;
  }
}
