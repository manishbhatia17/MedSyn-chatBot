import { Component, Input, Output, EventEmitter, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OptionModel } from 'src/model/optionModel';

@Component({
  selector: 'app-customer-options',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './customer-options.component.html',
  styleUrls: ['./customer-options.component.css']
})
export class CustomerOptionsComponent {
  @Input() message: string = '';
  @Output() optionSelected = new EventEmitter<OptionModel>();
  @Input() options: OptionModel[] = [];
  selectedOption:OptionModel | null = null;
 ngOnInit() {
 
 }



  select(option: OptionModel) {
    // UI only: set selected option for styling and notify parent
    this.selectedOption = option;
    this.optionSelected.emit(option);
    console.log('Selected option:', option);
  }
}
