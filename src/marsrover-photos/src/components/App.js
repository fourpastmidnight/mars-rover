import React from 'react';
import { Route } from 'react-router-dom';
import HomePage from './home/HomePage';
import PhotosPage from './photos/PhotosPage';
import './App.css';


const App = () => {
  return (
      <>
          <Route exact path="/"><HomePage numCarouselPhotos={4} /></Route>
          <Route path="/curiosity"><PhotosPage rover="curiosity" /></Route>
          <Route path="/spirit"><PhotosPage rover="spirit" /></Route>
          <Route path="/opportunity"><PhotosPage rover="opportunity" /></Route>
      </>
  )
}

export default App;
