import React from 'react';
import { Route } from 'react-router-dom';
import HomePage from './home/HomePage';
//import logo from '../logo.svg';
import './App.css';

// function App() {
//   return (
//     <div className="App">
//       <header className="App-header">
//         <img src={logo} className="App-logo" alt="logo" />
//         <p>
//           Edit <code>src/App.js</code> and save to reload.
//         </p>
//         <a
//           className="App-link"
//           href="https://reactjs.org"
//           target="_blank"
//           rel="noopener noreferrer"
//         >
//           Learn React
//         </a>
//       </header>
//     </div>
//   );
// }

const App = () => (
  <>
      <Route exact path="/"><HomePage numCarouselPhotos={4} /></Route>
      <Route path="/curiosity" />
      <Route path="/spirit" />
      <Route path="/opportunity" />
  </>
)

export default App;
