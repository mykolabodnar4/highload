CREATE TABLE IF NOT EXISTS  orders.items_by_category (
    id INT,
    name TEXT,
    category TEXT,
    price DECIMAL,
    manufacturer TEXT,
    short_description TEXT,
    custom_attributes MAP<TEXT, TEXT>,
    PRIMARY KEY ((category), price, id)
);

CREATE CUSTOM INDEX IF NOT EXISTS item_name_index 
ON orders.items_by_category (name) 
USING 'StorageAttachedIndex' 
WITH OPTIONS = { 'case_sensitive': 'false'};

CREATE INDEX IF NOT EXISTS item_manufacturer_index
ON orders.items_by_category (manufacturer) 
USING 'SAI' 
WITH OPTIONS = { 'case_sensitive': 'false'};

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (1, 'Galaxy S23 Ultra', 'Smartphone', 1199.00, 'Samsung', 'High-end Android phone with advanced camera features.', {'display': '6.8-inch AMOLED', 'camera': '200MP Triple Lens', 'battery': '5000mAh'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (2, 'iPhone 14 Pro Max', 'Smartphone', 1099.00, 'Apple', 'Premium smartphone with A16 Bionic chip and Dynamic Island.', {'display': '6.7-inch Super Retina', 'camera': '48MP Dual Lens', 'processor': 'A16 Bionic'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (3, 'Google Pixel 7 Pro', 'Smartphone', 899.00, 'Google', 'Flagship Android phone with AI-enhanced camera.', {'display': '6.7-inch OLED', 'camera': '50MP Dual Lens', 'software': 'Stock Android'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (4, 'MacBook Pro 14"', 'Laptop', 1999.00, 'Apple', 'Powerful laptop for professionals with M1 Pro chip.', {'processor': 'M1 Pro', 'storage': '512GB SSD', 'RAM': '16GB'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (5, 'Dell XPS 13 Plus', 'Laptop', 1399.00, 'Dell', 'Sleek ultrabook with edge-to-edge display.', {'processor': 'Intel i7-12700H', 'storage': '1TB SSD', 'weight': '2.71 lbs'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (6, 'HP Spectre x360', 'Laptop', 1249.00, 'HP', 'Convertible laptop with a vibrant touchscreen.', {'processor': 'Intel i5-1235U', 'RAM': '8GB', 'battery': '16 hours'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (7, 'Logitech MX Master 3S', 'Accessories', 99.00, 'Logitech', 'Ergonomic wireless mouse with precise control.', {'connectivity': 'Bluetooth/USB', 'dpi': '8000', 'battery_life': '70 days'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (9, 'Razer DeathAdder V3 Pro', 'Accessories', 149.00, 'Razer', 'Lightweight wireless gaming mouse with high precision.', {'connectivity': 'Wireless', 'weight': '63g', 'sensor': 'Focus Pro 30K'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (10, 'Keychron K8 Pro', 'Accessories', 119.00, 'Keychron', 'Wireless mechanical keyboard for professionals.', {'switch_type': 'Gateron Red', 'connectivity': 'Bluetooth/USB-C', 'backlighting': 'RGB'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (11, 'Echo Dot (5th Gen)', 'Smart Speaker', 49.00, 'Amazon', 'Compact smart speaker with Alexa voice assistant.', {'assistant': 'Alexa', 'audio_quality': 'High-Fidelity', 'connectivity': 'Wi-Fi/Bluetooth'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (12, 'Bose SoundLink Revolve+', 'Bluetooth Speaker', 299.00, 'Bose', 'Portable speaker with 360-degree sound.', {'battery_life': '16 hours', 'water_resistance': 'IP55', 'audio_quality': '360-degree sound'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (13, 'Sonos Move', 'Smart Speaker', 399.00, 'Sonos', 'Durable and portable smart speaker with voice control.', {'battery_life': '11 hours', 'connectivity': 'Wi-Fi/Bluetooth', 'audio_quality': 'Rich Bass'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (14, 'Instant Pot Duo', 'Kitchen Appliance', 129.00, 'Instant Brands', 'Multi-function pressure cooker for versatile cooking.', {'capacity': '6 quarts', 'functions': '7-in-1', 'material': 'Stainless Steel'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (15, 'Ninja Foodi Air Fryer', 'Kitchen Appliance', 169.00, 'Ninja', 'Versatile air fryer and pressure cooker combo.', {'capacity': '8 quarts', 'temperature_range': '105°F-450°F', 'functions': 'Pressure/Steam/Fry'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (16, 'Vitamix A3500', 'Kitchen Appliance', 599.00, 'Vitamix', 'High-performance blender for all your cooking needs.', {'power': '1500W', 'container_size': '64 oz', 'material': 'BPA-free Tritan'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (17, 'Yoga Mat Pro', 'Fitness Gear', 89.00, 'Manduka', 'High-quality, durable yoga mat for all practices.', {'material': 'PVC', 'thickness': '6mm', 'dimensions': '71x26 inches'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (18, 'Fitbit Charge 5', 'Fitness Tracker', 149.00, 'Fitbit', 'Advanced health and fitness tracker with GPS.', {'battery_life': '7 days', 'sensors': 'Heart Rate/SpO2/GPS', 'compatibility': 'iOS/Android'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (19, 'Theragun Prime', 'Fitness Gear', 299.00, 'Therabody', 'Percussion massage gun for muscle recovery.', {'speeds': '5', 'attachments': '4', 'battery_life': '2 hours'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (20, 'UltraBoost 22', 'Footwear', 180.00, 'Adidas', 'Lightweight running shoes with great cushioning.', {'material': 'Primeknit', 'sole': 'Boost Cushioning', 'weight': '10.9 oz'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (21, 'Nike Air Zoom Pegasus 39', 'Footwear', 130.00, 'Nike', 'Versatile running shoes for all levels of athletes.', {'material': 'Mesh', 'drop': '10mm', 'use_case': 'Daily Trainer'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (22, 'Timberland 6-Inch Boots', 'Footwear', 170.00, 'Timberland', 'Durable boots for outdoor adventures.', {'material': 'Nubuck Leather', 'waterproof': 'Yes', 'insulation': '200g PrimaLoft'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (23, 'PlayStation 5', 'Gaming Console', 499.00, 'Sony', 'Next-gen gaming console with stunning graphics.', {'storage': '825GB SSD', 'resolution': '4K HDR', 'controller': 'DualSense'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (24, 'Xbox Series X', 'Gaming Console', 499.00, 'Microsoft', 'High-performance gaming console with 4K capabilities.', {'storage': '1TB SSD', 'resolution': '4K HDR', 'compatibility': 'Backward-Compatible'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (25, 'Nintendo Switch OLED', 'Gaming Console', 349.00, 'Nintendo', 'Portable gaming console with vibrant OLED screen.', {'display': '7-inch OLED', 'storage': '64GB', 'mode': 'Handheld/Dock'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (26, 'LEGO Millennium Falcon', 'Toys', 159.00, 'LEGO', 'Detailed replica of the iconic Star Wars spaceship.', {'pieces': '7541', 'age_range': '16+', 'dimensions': '33x22x8 inches'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (27, 'Barbie Dreamhouse', 'Toys', 299.00, 'Mattel', 'Deluxe dollhouse with interactive features.', {'levels': '3', 'rooms': '8', 'accessories': '75+'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (28, 'Nerf Elite 2.0 Blaster', 'Toys', 69.00, 'Nerf', 'High-powered foam dart blaster for exciting battles.', {'range': '90 feet', 'darts_included': '36', 'firing_modes': 'Semi-Automatic'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (29, 'Kindle Paperwhite', 'E-Reader', 139.00, 'Amazon', 'Waterproof e-reader with adjustable warm light.', {'display': '6-inch E-Ink', 'storage': '8GB', 'battery_life': '10 weeks'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (30, 'Sony WH-1000XM5', 'Headphones', 399.00, 'Sony', 'Noise-canceling headphones with premium sound.', {'noise_cancellation': 'Adaptive', 'battery_life': '30 hours', 'connectivity': 'Bluetooth'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (31, 'AirPods Pro 2', 'Wireless Earbuds', 249.00, 'Apple', 'Noise-canceling earbuds with spatial audio.', {'noise_cancellation': 'Active', 'battery_life': '6 hours', 'connectivity': 'Lightning/USB-C'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (32, 'Nest Thermostat', 'Smart Home', 129.00, 'Google', 'Energy-saving smart thermostat with app control.', {'compatibility': 'HVAC Systems', 'control': 'App/Voice', 'power': 'Battery/Wired'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (33, 'Dyson V15 Detect', 'Vacuum Cleaner', 749.00, 'Dyson', 'Powerful cordless vacuum with laser dirt detection.', {'power': '230AW', 'dust_bin': '0.76L', 'filters': 'HEPA'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (34, 'Garmin Forerunner 255', 'Smartwatch', 349.00, 'Garmin', 'GPS running watch with advanced training features.', {'battery_life': '14 days', 'sensors': 'Heart Rate/SpO2/GPS', 'water_resistance': '5ATM'});

INSERT INTO orders.items_by_category (id, name, category, price, manufacturer, short_description, custom_attributes)
VALUES (35, 'TP-Link Deco X68', 'Wi-Fi Router', 279.00, 'TP-Link', 'Tri-band mesh Wi-Fi system for whole-home coverage.', {'speed': '3600Mbps', 'coverage': '6000 sq ft', 'connectivity': 'Wi-Fi 6'});
