DROP TABLE IF EXISTS #TempCodes
CREATE TABLE #TempCodes (
	CodeName VARCHAR(MAX) NOT NULL,
	CodeDescription VARCHAR(MAX) NOT NULL,
	CodeTypeID INT NOT NULL,
	IsRequired BIT NOT NULL,
)

-- STATES - 1
INSERT INTO #TempCodes VALUES
('AL', 'Alabama', 1, 0)
,('AK', 'Alaska', 1, 0)
,('AZ', 'Arizona', 1, 0)
,('AR', 'Arkansas', 1, 0)
,('CA', 'California', 1, 0)
,('CO', 'Colorado', 1, 0)
,('CT', 'Connecticut', 1, 0)
,('DE', 'Delaware', 1, 0)
,('FL', 'Florida', 1, 0)
,('GA', 'Georgia', 1, 0)
,('HI', 'Hawaii', 1, 0)
,('ID', 'Idaho', 1, 0)
,('IL', 'Illinois', 1, 1) -- required
,('IN', 'Indiana', 1, 0)
,('IA', 'Iowa', 1, 0)
,('KS', 'Kansas', 1, 0)
,('KY', 'Kentucky', 1, 0)
,('LA', 'Louisiana', 1, 0)
,('ME', 'Maine', 1, 0)
,('MD', 'Maryland', 1, 0)
,('MA', 'Massachusetts', 1, 0)
,('MI', 'Michigan', 1, 0)
,('MN', 'Minnesota', 1, 0)
,('MS', 'Mississippi', 1, 0)
,('MO', 'Missouri', 1, 0)
,('MT', 'Montana', 1, 0)
,('NE', 'Nebraska', 1, 0)
,('NV', 'Nevada', 1, 0)
,('NH', 'New Hampshire', 1, 0)
,('NJ', 'New Jersey', 1, 0)
,('NM', 'New Mexico', 1, 0)
,('NY', 'New York', 1, 0)
,('NC', 'North Carolina', 1, 0)
,('ND', 'North Dakota', 1, 0)
,('OH', 'Ohio', 1, 0)
,('OK', 'Oklahoma', 1, 0)
,('OR', 'Oregon', 1, 0)
,('PA', 'Pennsylvania', 1, 0)
,('RI', 'Rhode Island', 1, 0)
,('SC', 'South Carolina', 1, 0)
,('SD', 'South Dakota', 1, 0)
,('TN', 'Tennessee', 1, 0)
,('TX', 'Texas', 1, 0)
,('UT', 'Utah', 1, 0)
,('VT', 'Vermont', 1, 0)
,('VA', 'Virginia', 1, 0)
,('WA', 'Washington', 1, 0)
,('WV', 'West Virginia', 1, 0)
,('WI', 'Wisconsin', 1, 0)
,('WY', 'Wyoming', 1, 0)
,('AB', 'Alberta', 1, 0)
,('BC', 'British Columbia', 1, 0)
,('MB', 'Manitoba', 1, 0)
,('NB', 'New Brunswick', 1, 0)
,('NL', 'Newfoundland and Labrador', 1, 0)
,('NT', 'Northwest Territories', 1, 0)
,('NS', 'Nova Scotia', 1, 0)
,('NU', 'Nunavut', 1, 0)
,('ON', 'Ontario', 1, 0)
,('PE', 'Prince Edward Island', 1, 0)
,('QC', 'Quebec', 1, 0)
,('SK', 'Saskatchewan', 1, 0)
,('YT', 'Yukon', 1, 0)


-- COUNTRIES - 2
INSERT INTO #TempCodes VALUES
('AFG', 'Afghanistan', 2, 0)
,('ALA', 'Åland Islands', 2, 0)
,('ALB', 'Albania', 2, 0)
,('DZA', 'Algeria', 2, 0)
,('ASM', 'American Samoa', 2, 0)
,('AND', 'Andorra', 2, 0)
,('AGO', 'Angola', 2, 0)
,('AIA', 'Anguilla', 2, 0)
,('ATA', 'Antarctica', 2, 0)
,('ATG', 'Antigua and Barbuda', 2, 0)
,('ARG', 'Argentina', 2, 0)
,('ARM', 'Armenia', 2, 0)
,('ABW', 'Aruba', 2, 0)
,('AUS', 'Australia', 2, 0)
,('AUT', 'Austria', 2, 0)
,('AZE', 'Azerbaijan', 2, 0)
,('BHS', 'Bahamas (the)', 2, 0)
,('BHR', 'Bahrain', 2, 0)
,('BGD', 'Bangladesh', 2, 0)
,('BRB', 'Barbados', 2, 0)
,('BLR', 'Belarus', 2, 0)
,('BEL', 'Belgium', 2, 0)
,('BLZ', 'Belize', 2, 0)
,('BEN', 'Benin', 2, 0)
,('BMU', 'Bermuda', 2, 0)
,('BTN', 'Bhutan', 2, 0)
,('BOL', 'Bolivia (Plurinational State of)', 2, 0)
,('BES', 'Bonaire, Sint Eustatius and Saba', 2, 0)
,('BIH', 'Bosnia and Herzegovina', 2, 0)
,('BWA', 'Botswana', 2, 0)
,('BVT', 'Bouvet Island', 2, 0)
,('BRA', 'Brazil', 2, 0)
,('IOT', 'British Indian Ocean Territory (the)', 2, 0)
,('BRN', 'Brunei Darussalam', 2, 0)
,('BGR', 'Bulgaria', 2, 0)
,('BFA', 'Burkina Faso', 2, 0)
,('BDI', 'Burundi', 2, 0)
,('CPV', 'Cabo Verde', 2, 0)
,('KHM', 'Cambodia', 2, 0)
,('CMR', 'Cameroon', 2, 0)
,('CAN', 'Canada', 2, 0)
,('CYM', 'Cayman Islands (the)', 2, 0)
,('CAF', 'Central African Republic (the)', 2, 0)
,('TCD', 'Chad', 2, 0)
,('CHL', 'Chile', 2, 0)
,('CHN', 'China', 2, 0)
,('CXR', 'Christmas Island', 2, 0)
,('CCK', 'Cocos (Keeling) Islands (the)', 2, 0)
,('COL', 'Colombia', 2, 0)
,('COM', 'Comoros (the)', 2, 0)
,('COD', 'Congo (the Democratic Republic of the)', 2, 0)
,('COG', 'Congo (the)', 2, 0)
,('COK', 'Cook Islands (the)', 2, 0)
,('CRI', 'Costa Rica', 2, 0)
,('CIV', 'Côte d''Ivoire', 2, 0)
,('HRV', 'Croatia', 2, 0)
,('CUB', 'Cuba', 2, 0)
,('CUW', 'Curaçao', 2, 0)
,('CYP', 'Cyprus', 2, 0)
,('CZE', 'Czechia', 2, 0)
,('DNK', 'Denmark', 2, 0)
,('DJI', 'Djibouti', 2, 0)
,('DMA', 'Dominica', 2, 0)
,('DOM', 'Dominican Republic (the)', 2, 0)
,('ECU', 'Ecuador', 2, 0)
,('EGY', 'Egypt', 2, 0)
,('SLV', 'El Salvador', 2, 0)
,('GNQ', 'Equatorial Guinea', 2, 0)
,('ERI', 'Eritrea', 2, 0)
,('EST', 'Estonia', 2, 0)
,('SWZ', 'Eswatini', 2, 0)
,('ETH', 'Ethiopia', 2, 0)
,('FLK', 'Falkland Islands (the) [Malvinas]', 2, 0)
,('FRO', 'Faroe Islands (the)', 2, 0)
,('FJI', 'Fiji', 2, 0)
,('FIN', 'Finland', 2, 0)
,('FRA', 'France', 2, 0)
,('GUF', 'French Guiana', 2, 0)
,('PYF', 'French Polynesia', 2, 0)
,('ATF', 'French Southern Territories (the)', 2, 0)
,('GAB', 'Gabon', 2, 0)
,('GMB', 'Gambia (the)', 2, 0)
,('GEO', 'Georgia', 2, 0)
,('DEU', 'Germany', 2, 0)
,('GHA', 'Ghana', 2, 0)
,('GIB', 'Gibraltar', 2, 0)
,('GRC', 'Greece', 2, 0)
,('GRL', 'Greenland', 2, 0)
,('GRD', 'Grenada', 2, 0)
,('GLP', 'Guadeloupe', 2, 0)
,('GUM', 'Guam', 2, 0)
,('GTM', 'Guatemala', 2, 0)
,('GGY', 'Guernsey', 2, 0)
,('GIN', 'Guinea', 2, 0)
,('GNB', 'Guinea-Bissau', 2, 0)
,('GUY', 'Guyana', 2, 0)
,('HTI', 'Haiti', 2, 0)
,('HMD', 'Heard Island and McDonald Islands', 2, 0)
,('VAT', 'Holy See (the)', 2, 0)
,('HND', 'Honduras', 2, 0)
,('HKG', 'Hong Kong', 2, 0)
,('HUN', 'Hungary', 2, 0)
,('ISL', 'Iceland', 2, 0)
,('IND', 'India', 2, 0)
,('IDN', 'Indonesia', 2, 0)
,('IRN', 'Iran (Islamic Republic of)', 2, 0)
,('IRQ', 'Iraq', 2, 0)
,('IRL', 'Ireland', 2, 0)
,('IMN', 'Isle of Man', 2, 0)
,('ISR', 'Israel', 2, 0)
,('ITA', 'Italy', 2, 0)
,('JAM', 'Jamaica', 2, 0)
,('JPN', 'Japan', 2, 0)
,('JEY', 'Jersey', 2, 0)
,('JOR', 'Jordan', 2, 0)
,('KAZ', 'Kazakhstan', 2, 0)
,('KEN', 'Kenya', 2, 0)
,('KIR', 'Kiribati', 2, 0)
,('PRK', 'Korea (the Democratic People''s Republic of)', 2, 0)
,('KOR', 'Korea (the Republic of)', 2, 0)
,('KWT', 'Kuwait', 2, 0)
,('KGZ', 'Kyrgyzstan', 2, 0)
,('LAO', 'Lao People''s Democratic Republic (the)', 2, 0)
,('LVA', 'Latvia', 2, 0)
,('LBN', 'Lebanon', 2, 0)
,('LSO', 'Lesotho', 2, 0)
,('LBR', 'Liberia', 2, 0)
,('LBY', 'Libya', 2, 0)
,('LIE', 'Liechtenstein', 2, 0)
,('LTU', 'Lithuania', 2, 0)
,('LUX', 'Luxembourg', 2, 0)
,('MAC', 'Macao', 2, 0)
,('MDG', 'Madagascar', 2, 0)
,('MWI', 'Malawi', 2, 0)
,('MYS', 'Malaysia', 2, 0)
,('MDV', 'Maldives', 2, 0)
,('MLI', 'Mali', 2, 0)
,('MLT', 'Malta', 2, 0)
,('MHL', 'Marshall Islands (the)', 2, 0)
,('MTQ', 'Martinique', 2, 0)
,('MRT', 'Mauritania', 2, 0)
,('MUS', 'Mauritius', 2, 0)
,('MYT', 'Mayotte', 2, 0)
,('MEX', 'Mexico', 2, 0)
,('FSM', 'Micronesia (Federated States of)', 2, 0)
,('MDA', 'Moldova (the Republic of)', 2, 0)
,('MCO', 'Monaco', 2, 0)
,('MNG', 'Mongolia', 2, 0)
,('MNE', 'Montenegro', 2, 0)
,('MSR', 'Montserrat', 2, 0)
,('MAR', 'Morocco', 2, 0)
,('MOZ', 'Mozambique', 2, 0)
,('MMR', 'Myanmar', 2, 0)
,('NAM', 'Namibia', 2, 0)
,('NRU', 'Nauru', 2, 0)
,('NPL', 'Nepal', 2, 0)
,('NLD', 'Netherlands (the)', 2, 0)
,('NCL', 'New Caledonia', 2, 0)
,('NZL', 'New Zealand', 2, 0)
,('NIC', 'Nicaragua', 2, 0)
,('NER', 'Niger (the)', 2, 0)
,('NGA', 'Nigeria', 2, 0)
,('NIU', 'Niue', 2, 0)
,('NFK', 'Norfolk Island', 2, 0)
,('MNP', 'Northern Mariana Islands (the)', 2, 0)
,('NOR', 'Norway', 2, 0)
,('OMN', 'Oman', 2, 0)
,('PAK', 'Pakistan', 2, 0)
,('PLW', 'Palau', 2, 0)
,('PSE', 'Palestine, State of', 2, 0)
,('PAN', 'Panama', 2, 0)
,('PNG', 'Papua New Guinea', 2, 0)
,('PRY', 'Paraguay', 2, 0)
,('PER', 'Peru', 2, 0)
,('PHL', 'Philippines (the)', 2, 0)
,('PCN', 'Pitcairn', 2, 0)
,('POL', 'Poland', 2, 0)
,('PRT', 'Portugal', 2, 0)
,('PRI', 'Puerto Rico', 2, 0)
,('QAT', 'Qatar', 2, 0)
,('MKD', 'Republic of North Macedonia', 2, 0)
,('REU', 'Réunion', 2, 0)
,('ROU', 'Romania', 2, 0)
,('RUS', 'Russian Federation (the)', 2, 0)
,('RWA', 'Rwanda', 2, 0)
,('BLM', 'Saint Barthélemy', 2, 0)
,('SHN', 'Saint Helena, Ascension and Tristan da Cunha', 2, 0)
,('KNA', 'Saint Kitts and Nevis', 2, 0)
,('LCA', 'Saint Lucia', 2, 0)
,('MAF', 'Saint Martin (French part)', 2, 0)
,('SPM', 'Saint Pierre and Miquelon', 2, 0)
,('VCT', 'Saint Vincent and the Grenadines', 2, 0)
,('WSM', 'Samoa', 2, 0)
,('SMR', 'San Marino', 2, 0)
,('STP', 'Sao Tome and Principe', 2, 0)
,('SAU', 'Saudi Arabia', 2, 0)
,('SEN', 'Senegal', 2, 0)
,('SRB', 'Serbia', 2, 0)
,('SYC', 'Seychelles', 2, 0)
,('SLE', 'Sierra Leone', 2, 0)
,('SGP', 'Singapore', 2, 0)
,('SXM', 'Sint Maarten (Dutch part)', 2, 0)
,('SVK', 'Slovakia', 2, 0)
,('SVN', 'Slovenia', 2, 0)
,('SLB', 'Solomon Islands', 2, 0)
,('SOM', 'Somalia', 2, 0)
,('ZAF', 'South Africa', 2, 0)
,('SGS', 'South Georgia and the South Sandwich Islands', 2, 0)
,('SSD', 'South Sudan', 2, 0)
,('ESP', 'Spain', 2, 0)
,('LKA', 'Sri Lanka', 2, 0)
,('SDN', 'Sudan (the)', 2, 0)
,('SUR', 'Suriname', 2, 0)
,('SJM', 'Svalbard and Jan Mayen', 2, 0)
,('SWE', 'Sweden', 2, 0)
,('CHE', 'Switzerland', 2, 0)
,('SYR', 'Syrian Arab Republic', 2, 0)
,('TWN', 'Taiwan (Province of China)', 2, 0)
,('TJK', 'Tajikistan', 2, 0)
,('TZA', 'Tanzania, United Republic of', 2, 0)
,('THA', 'Thailand', 2, 0)
,('TLS', 'Timor-Leste', 2, 0)
,('TGO', 'Togo', 2, 0)
,('TKL', 'Tokelau', 2, 0)
,('TON', 'Tonga', 2, 0)
,('TTO', 'Trinidad and Tobago', 2, 0)
,('TUN', 'Tunisia', 2, 0)
,('TUR', 'Turkey', 2, 0)
,('TKM', 'Turkmenistan', 2, 0)
,('TCA', 'Turks and Caicos Islands (the)', 2, 0)
,('TUV', 'Tuvalu', 2, 0)
,('UGA', 'Uganda', 2, 0)
,('UKR', 'Ukraine', 2, 0)
,('ARE', 'United Arab Emirates (the)', 2, 0)
,('GBR', 'United Kingdom of Great Britain and Northern Ireland (the)', 2, 0)
,('UMI', 'United States Minor Outlying Islands (the)', 2, 0)
,('USA', 'United States of America (the)', 2, 1) -- required
,('URY', 'Uruguay', 2, 0)
,('UZB', 'Uzbekistan', 2, 0)
,('VUT', 'Vanuatu', 2, 0)
,('VEN', 'Venezuela (Bolivarian Republic of)', 2, 0)
,('VNM', 'Viet Nam', 2, 0)
,('VGB', 'Virgin Islands (British)', 2, 0)
,('VIR', 'Virgin Islands (U.S.)', 2, 0)
,('WLF', 'Wallis and Futuna', 2, 0)
,('ESH', 'Western Sahara', 2, 0)
,('YEM', 'Yemen', 2, 0)
,('ZMB', 'Zambia', 2, 0)
,('ZWE', 'Zimbabwe', 2, 0)
,('EARTH', 'Earth', 2, 0)


-- VENDOR GL - 3 removed


-- U/M - 4
INSERT INTO #TempCodes VALUES
('Each', 'Each', 4, 0)

-- SHIP WEIGHT UNIT - 5
INSERT INTO #TempCodes VALUES
('LB', 'LB', 5, 1)
,('KG', 'KG', 5, 1)

-- SHIP DIMENSIONS UNIT - 6
INSERT INTO #TempCodes VALUES
('inches', 'IN', 6, 1)
,('centimeters', 'CM', 6, 1)

-- CUSTOMER GL - 7 REMOVED


-- CUSTOMER PRACTICE TYPE - 8
INSERT INTO #TempCodes VALUES
('Clinic', 'Clinic', 8, 0)
,('Private Practice', 'Private Practice', 8, 0)
,('Hospital', 'Hospital', 8, 0)
,('SANE/CAC', 'SANE/CAC', 8, 0)
,('Distributor', 'Distributor', 8, 0)
,('NGO', 'NGO', 8, 0)


-- CUSTOMER SALES TAX - 9
INSERT INTO #TempCodes VALUES
('Exempt', 'Exempt', 9, 0),
('IL', 'Illinois', 9, 0)


-- CUSTOMER STATUS - 10
INSERT INTO #TempCodes VALUES
('Active', 'Active', 10, 1), -- required
('Inactive', 'Inactive', 10, 1), -- required
('CreditHold', 'Credit Hold', 10, 1) -- required

-- FEDEX SHIP METHOD - 11
INSERT INTO #TempCodes VALUES
--  ('SAME_DAY_METRO_AFTERNOON', 'SameDay METRO_AFTERNOON', 11, 1)
-- ,('SAME_DAY_METRO_MORNING', 'SameDay METRO_MORNING', 11, 1)
-- ,('SAME_DAY_METRO_RUSH', 'SameDay METRO_RUSH', 11, 1)
('fedex_first_overnight', 'Fedex First Overnight', 11, 1)
,('fedex_priority_overnight', 'Fedex Priority Overnight', 11, 1)
,('fedex_standard_overnight', 'FEDEX Standard Overnight', 11, 1)
,('fedex_2day', 'FEDEX 2DAY', 11, 1)
,('fedex_2day_am', 'FEDEX 2DAY AM', 11, 1)
,('fedex_express_saver', 'FEDEX Express Saver', 11, 1)
,('fedex_ground', 'FEDEX Ground', 11, 1)
,('fedex_international_economy', 'INTERNATIONAL_ECONOMY', 11, 1)
,('fedex_international_priority', 'INTERNATIONAL_PRIORITY', 11, 1)

-- UPS SHIP METHOD - 12
INSERT INTO #TempCodes VALUES
 ('ups_next_day_air', 'Next Day Air', 12, 1)
,('ups_2nd_day_air', '2nd Day Air', 12, 1)
,('ups_ground', 'Ground', 12, 1)
,('ups_worldwide_express', 'Worldwide Express', 12, 1)
,('ups_worldwide_expedited', 'Worldwide Expedited', 12, 1)
,('ups_3_day_select', '3 Day Select', 12, 1)
,('ups_next_day_air_saver', 'Next Day Air Saver', 12, 1)
,('ups_next_day_air_early_am', 'Next Day Air Early', 12, 1)
,('ups_worldwide_express_plus', 'Worldwide Express Plus', 12, 1)
,('ups_2nd_day_air_am', '2nd Day Air A.M.', 12, 1)
,('ups_worldwide_saver', 'Worldwide Saver (Express)', 12, 1)
,('ups_standard_international', 'UPS Standard', 12, 1)

-- OTHER SHIP METHOD - 13
INSERT INTO #TempCodes VALUES
('Ground', 'Ground', 13, 0)

-- 14 removed

-- INVENTORY ADJUSTMENT REASON - 15
INSERT INTO #TempCodes VALUES
('Breakage', 'Breakage', 15, 0)
,('Shrinkage', 'Shrinkage', 15, 0)
,('Return', 'Return', 15, 0)
,('Write-off', 'Write-off', 15, 0)

-- SHIPPING COMPANIES - 16
INSERT INTO #TempCodes VALUES
('FedEx', 'FedEx', 16, 1) -- required
,('UPS', 'UPS', 16, 1) -- required
,('FedEx - MedGyn', 'FedEx - MedGyn', 16, 1) -- required
,('UPS - MedGyn', 'UPS - MedGyn', 16, 1) -- required
,('UPS Freight', 'UPS Freight', 16, 1) -- required

-- VENDOR PURCHASES GL - 17
INSERT INTO #TempCodes VALUES
('40500-01', '40500-01', 17, 0)
,('40500-03', '40500-03', 17, 0)
,('40500-04', '40500-04', 17, 0)

-- VENDOR FREIGHT CHARGES GL - 18
INSERT INTO #TempCodes VALUES
('42000-01', '42000-01', 18, 0)
,('42000-03', '42000-03', 18, 0)
,('42000-04', '42000-04', 18, 0)

-- VENDOR ACCOUNTS PAYABLE GL - 19
INSERT INTO #TempCodes VALUES
('20500-01', '20500-01', 19, 0)
,('20500-03', '20500-03', 19, 0)
,('20500-04', '20500-04', 19, 0)

-- CUSTOMER SALES GL - 20
INSERT INTO #TempCodes VALUES
('30500-01', '30500-01', 20, 0)
,('30600-03', '30600-03', 20, 0)
,('30700-04', '30700-04', 20, 0)

-- CUSTOMER SHIPPING CHARGE GL - 21
INSERT INTO #TempCodes VALUES
('34000-01', '34000-01', 21, 0)
,('34000-03', '34000-03', 21, 0)
,('34000-04', '34000-04', 21, 0)

-- CUSTOMER RECEIVABLE GL - 22
INSERT INTO #TempCodes VALUES
('12000-01', '12000-01', 22, 0)
,('12000-03', '12000-03', 22, 0)
,('12000-04', '12000-04', 22, 0)

-- PAYMENT TYPE - 23
INSERT INTO #TempCodes VALUES
('Check', 'Check', 23, 0)
,('CreditCard', 'Credit Card', 23, 0)
,('ACH', 'ACH', 23, 0)
,('EDI', 'EDI', 23, 0)
,('Wire', 'Wire', 23, 0)
,('Paypal', 'Paypal', 23, 0)

-- VENDOR STATUS - 24
INSERT INTO #TempCodes VALUES
('Active', 'Active', 24, 1) -- required
,('Inactive', 'Inactive', 24, 1) -- required

MERGE Code as target
	USING(SELECT * FROM #TempCodes) as source
		ON source.CodeName = target.CodeName AND source.CodeTypeID = target.CodeTypeID
	WHEN MATCHED THEN
		UPDATE SET
			IsRequired = source.IsRequired
	WHEN NOT MATCHED BY TARGET THEN
		INSERT VALUES (
			source.CodeName
			,source.CodeDescription
			,source.CodeTypeID
			,0
			,source.IsRequired
		)
	WHEN NOT MATCHED BY SOURCE THEN
		UPDATE SET
			IsDeleted = 1;

DROP TABLE #TempCodes
